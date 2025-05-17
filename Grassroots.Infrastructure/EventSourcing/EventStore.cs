using Grassroots.Application.Common.Interfaces;
using Grassroots.Domain.Events;
using Grassroots.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Grassroots.Infrastructure.EventSourcing
{
    /// <summary>
    /// 事件存储实现
    /// 为事件溯源提供持久化机制，负责保存和读取聚合的事件流
    /// 事件存储是事件溯源架构的核心组件，它将所有状态变更作为事件序列保存
    /// 实现了以下功能：
    /// 1. 保存聚合产生的新事件
    /// 2. 加载聚合的完整事件历史
    /// 3. 支持从特定版本开始加载事件
    /// 4. 确保事件的并发安全性
    /// </summary>
    public class EventStore : IEventStore
    {
        private readonly EventStoreDbContext _context;
        private readonly IDomainEventService _domainEventService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">事件存储数据库上下文</param>
        /// <param name="domainEventService">领域事件服务，用于发布已保存的事件</param>
        public EventStore(EventStoreDbContext context, IDomainEventService domainEventService)
        {
            _context = context;
            _domainEventService = domainEventService;
        }

        /// <summary>
        /// 保存事件
        /// 将聚合产生的新事件持久化到事件存储
        /// 包含乐观并发控制，确保不会覆盖其他事务的更改
        /// </summary>
        /// <param name="aggregateId">聚合ID</param>
        /// <param name="events">待保存的事件列表</param>
        /// <param name="expectedVersion">期望的聚合版本，用于并发控制</param>
        /// <returns>表示异步操作的任务</returns>
        /// <exception cref="ConcurrencyException">当聚合版本与期望版本不匹配时抛出</exception>
        public async Task SaveEventsAsync(string aggregateId, IEnumerable<IDomainEvent> events, int expectedVersion)
        {
            // 获取聚合的事件存储记录
            var eventDescriptors = await _context.EventDescriptors
                .Where(x => x.AggregateId == aggregateId)
                .OrderBy(x => x.Version)
                .ToListAsync();

            // 检查版本冲突
            // expectedVersion为-1表示新聚合，否则必须匹配当前最大版本
            if (expectedVersion != -1 && eventDescriptors.Any() && eventDescriptors.Max(x => x.Version) != expectedVersion)
            {
                throw new ConcurrencyException($"Aggregate {aggregateId} has been modified outside of this transaction");
            }

            var version = expectedVersion;

            // 保存每个事件
            foreach (var @event in events)
            {
                version++;
                var eventType = @event.GetType().AssemblyQualifiedName;
                var serializedData = JsonSerializer.Serialize(@event);
                
                // 创建事件描述符对象
                var eventDescriptor = new EventDescriptor
                {
                    AggregateId = aggregateId,
                    Version = version,
                    Timestamp = @event.OccurredOn,
                    EventType = eventType,
                    EventData = serializedData
                };
                
                // 添加到数据库上下文
                await _context.EventDescriptors.AddAsync(eventDescriptor);
                
                // 通过领域事件服务发布事件，以便其他订阅者可以响应
                await _domainEventService.PublishAsync(@event);
            }
            
            // 提交所有更改
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 获取事件
        /// 加载特定聚合的完整事件历史
        /// </summary>
        /// <param name="aggregateId">聚合ID</param>
        /// <returns>按版本排序的事件列表</returns>
        public async Task<List<IDomainEvent>> GetEventsAsync(string aggregateId)
        {
            var eventDescriptors = await _context.EventDescriptors
                .Where(x => x.AggregateId == aggregateId)
                .OrderBy(x => x.Version)
                .ToListAsync();
            
            return DeserializeEvents(eventDescriptors);
        }

        /// <summary>
        /// 获取事件流
        /// 加载特定聚合从指定版本开始的事件历史
        /// 用于增量加载事件，例如在事件流播放或快照恢复场景
        /// </summary>
        /// <param name="aggregateId">聚合ID</param>
        /// <param name="fromVersion">起始版本（不包含此版本）</param>
        /// <returns>按版本排序的事件列表</returns>
        public async Task<List<IDomainEvent>> GetEventsAsync(string aggregateId, int fromVersion)
        {
            var eventDescriptors = await _context.EventDescriptors
                .Where(x => x.AggregateId == aggregateId && x.Version > fromVersion)
                .OrderBy(x => x.Version)
                .ToListAsync();
            
            return DeserializeEvents(eventDescriptors);
        }

        /// <summary>
        /// 反序列化事件
        /// 将存储的事件描述符转换回领域事件对象
        /// </summary>
        /// <param name="eventDescriptors">事件描述符列表</param>
        /// <returns>领域事件列表</returns>
        private List<IDomainEvent> DeserializeEvents(List<EventDescriptor> eventDescriptors)
        {
            var events = new List<IDomainEvent>();
            
            foreach (var descriptor in eventDescriptors)
            {
                // 通过完全限定名称获取事件类型
                var eventType = Type.GetType(descriptor.EventType);
                if (eventType == null) continue;
                
                // 反序列化事件数据
                var @event = JsonSerializer.Deserialize(descriptor.EventData, eventType) as IDomainEvent;
                if (@event == null) continue;
                
                events.Add(@event);
            }
            
            return events;
        }
    }
    
    /// <summary>
    /// 并发异常
    /// 当尝试保存事件时检测到版本冲突时抛出
    /// 这表明聚合已被其他事务修改
    /// </summary>
    public class ConcurrencyException : Exception
    {
        public ConcurrencyException(string message) : base(message)
        {
        }
    }
} 