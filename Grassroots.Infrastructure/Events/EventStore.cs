using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Grassroots.Domain.Events;
using Grassroots.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Grassroots.Infrastructure.Events
{
    /// <summary>
    /// 事件存储实现
    /// </summary>
    public class EventStore : IEventStore
    {
        private readonly AppDbContext _dbContext;
        private readonly IDomainEventBus _eventBus;

        public EventStore(AppDbContext dbContext, IDomainEventBus eventBus)
        {
            _dbContext = dbContext;
            _eventBus = eventBus;
        }

        /// <summary>
        /// 保存事件
        /// </summary>
        /// <param name="aggregateId">聚合根ID</param>
        /// <param name="events">事件列表</param>
        /// <param name="expectedVersion">期望版本</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>任务</returns>
        public async Task SaveEventsAsync(string aggregateId, IEnumerable<DomainEvent> events, int expectedVersion, CancellationToken cancellationToken = default)
        {
            var eventList = events.ToList();
            if (!eventList.Any())
            {
                return;
            }

            // 检查并发
            var lastEvent = await _dbContext.EventStore
                .Where(e => e.AggregateId == aggregateId)
                .OrderByDescending(e => e.Version)
                .FirstOrDefaultAsync(cancellationToken);

            var lastVersion = lastEvent?.Version ?? 0;
            if (lastVersion != expectedVersion)
            {
                throw new InvalidOperationException($"Concurrency conflict. Expected version {expectedVersion}, but got {lastVersion}");
            }

            var version = expectedVersion;
            var eventModels = new List<EventStoreEntity>();

            foreach (var @event in eventList)
            {
                version++;
                var eventData = JsonSerializer.Serialize(@event);
                var eventModel = new EventStoreEntity
                {
                    Id = Guid.NewGuid(),
                    AggregateId = aggregateId,
                    AggregateType = @event.AggregateType,
                    EventType = @event.EventType,
                    Version = version,
                    Data = eventData,
                    Metadata = @event.Metadata.RootElement.GetRawText(),
                    CreatedAt = @event.OccurredOn
                };

                eventModels.Add(eventModel);
            }

            await _dbContext.EventStore.AddRangeAsync(eventModels, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            // 发布事件
            foreach (var @event in eventList)
            {
                await _eventBus.PublishAsync(@event, cancellationToken);
            }
        }

        /// <summary>
        /// 获取聚合根的事件
        /// </summary>
        /// <param name="aggregateId">聚合根ID</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>事件列表</returns>
        public async Task<List<DomainEvent>> GetEventsAsync(string aggregateId, CancellationToken cancellationToken = default)
        {
            var eventModels = await _dbContext.EventStore
                .Where(e => e.AggregateId == aggregateId)
                .OrderBy(e => e.Version)
                .ToListAsync(cancellationToken);

            return DeserializeEvents(eventModels);
        }

        /// <summary>
        /// 获取聚合根的事件
        /// </summary>
        /// <param name="aggregateId">聚合根ID</param>
        /// <param name="fromVersion">起始版本</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>事件列表</returns>
        public async Task<List<DomainEvent>> GetEventsAsync(string aggregateId, int fromVersion, CancellationToken cancellationToken = default)
        {
            var eventModels = await _dbContext.EventStore
                .Where(e => e.AggregateId == aggregateId && e.Version >= fromVersion)
                .OrderBy(e => e.Version)
                .ToListAsync(cancellationToken);

            return DeserializeEvents(eventModels);
        }

        private List<DomainEvent> DeserializeEvents(List<EventStoreEntity> eventModels)
        {
            var domainEvents = new List<DomainEvent>();

            foreach (var eventModel in eventModels)
            {
                var eventType = Type.GetType(eventModel.EventType);
                if (eventType == null)
                {
                    // 尝试从当前程序集加载类型
                    var domainAssembly = typeof(DomainEvent).Assembly;
                    eventType = domainAssembly.GetTypes().FirstOrDefault(t => t.Name == eventModel.EventType);
                    
                    if (eventType == null)
                    {
                        // 无法找到事件类型，跳过
                        continue;
                    }
                }

                var @event = (DomainEvent)JsonSerializer.Deserialize(eventModel.Data, eventType);
                domainEvents.Add(@event);
            }

            return domainEvents;
        }
    }
} 