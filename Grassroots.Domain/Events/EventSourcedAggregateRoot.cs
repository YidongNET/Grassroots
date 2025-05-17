using System;
using System.Collections.Generic;
using System.Linq;

namespace Grassroots.Domain.Events
{
    /// <summary>
    /// 事件溯源聚合根基类
    /// 实现事件溯源(Event Sourcing)模式，通过事件重放来重建聚合状态
    /// 聚合的当前状态完全由其历史事件序列决定，而不是直接存储状态
    /// 这使得系统可以:
    /// 1. 重现任意时间点的聚合状态
    /// 2. 维护完整的审计日志
    /// 3. 支持复杂的业务场景和时间点查询
    /// </summary>
    public abstract class EventSourcedAggregateRoot : IEventSourcedAggregate
    {
        private readonly List<IDomainEvent> _uncommittedEvents = new();
        private readonly Dictionary<Type, Action<IDomainEvent>> _eventHandlers = new();

        /// <summary>
        /// 聚合ID
        /// 每个事件溯源聚合的唯一标识符
        /// </summary>
        public abstract string Id { get; protected set; }
        
        /// <summary>
        /// 聚合版本
        /// 用于乐观并发控制，每应用一个事件时递增
        /// 初始值为-1表示聚合尚未初始化
        /// </summary>
        public int Version { get; protected set; } = -1;

        /// <summary>
        /// 构造函数
        /// 初始化聚合并注册事件处理器
        /// </summary>
        protected EventSourcedAggregateRoot()
        {
            RegisterEventHandlers();
        }
        
        /// <summary>
        /// 获取未提交的事件
        /// 返回尚未持久化的事件集合，这些事件代表聚合的待保存状态变更
        /// </summary>
        /// <returns>未提交的事件集合</returns>
        public IEnumerable<IDomainEvent> GetUncommittedEvents()
        {
            return _uncommittedEvents.AsEnumerable();
        }
        
        /// <summary>
        /// 应用事件到聚合
        /// 执行事件处理逻辑，修改聚合状态，并将事件标记为未提交
        /// </summary>
        /// <param name="event">要应用的领域事件</param>
        public void Apply(IDomainEvent @event)
        {
            ApplyEvent(@event, true);
        }
        
        /// <summary>
        /// 标记事件已提交
        /// 在事件成功持久化后调用，清空未提交事件列表
        /// </summary>
        public void MarkEventsAsCommitted()
        {
            _uncommittedEvents.Clear();
        }
        
        /// <summary>
        /// 加载事件历史
        /// 通过按顺序重放历史事件来重建聚合状态
        /// 这是事件溯源模式的核心机制
        /// </summary>
        /// <param name="history">事件历史序列</param>
        public void LoadFromHistory(IEnumerable<IDomainEvent> history)
        {
            foreach (var @event in history)
            {
                ApplyEvent(@event, false);
                Version = @event.Version;
            }
        }
        
        /// <summary>
        /// 注册事件处理器
        /// 子类必须实现此方法来注册所有支持的事件类型的处理方法
        /// </summary>
        protected abstract void RegisterEventHandlers();
        
        /// <summary>
        /// 注册事件处理方法
        /// 为特定事件类型注册处理函数，处理函数负责更新聚合状态
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="handler">处理该类型事件的方法</param>
        protected void RegisterHandler<T>(Action<T> handler) where T : IDomainEvent
        {
            _eventHandlers[typeof(T)] = @event => handler((T)@event);
        }
        
        /// <summary>
        /// 应用事件
        /// 内部方法，根据事件类型调用相应的处理器，并根据isNew参数决定是否将事件加入未提交列表
        /// </summary>
        /// <param name="event">要应用的事件</param>
        /// <param name="isNew">是否新事件(true)还是历史事件(false)</param>
        /// <exception cref="InvalidOperationException">当找不到事件处理器时抛出</exception>
        private void ApplyEvent(IDomainEvent @event, bool isNew)
        {
            if (_eventHandlers.TryGetValue(@event.GetType(), out var handler))
            {
                handler(@event);
                
                if (isNew)
                {
                    Version++;
                    _uncommittedEvents.Add(@event);
                }
            }
            else
            {
                throw new InvalidOperationException($"No handler registered for {@event.GetType().Name}");
            }
        }
    }
} 