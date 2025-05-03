using System;
using System.Collections.Generic;
using Grassroots.Domain.Entity;
using Grassroots.Domain.Events;

namespace Grassroots.Domain.AggregateRoots
{
    /// <summary>
    /// 聚合根基类
    /// </summary>
    public abstract class AggregateRoot : BaseEntity
    {
        private readonly List<DomainEvent> _domainEvents = new List<DomainEvent>();
        
        /// <summary>
        /// 聚合根版本号
        /// </summary>
        public int Version { get; protected set; } = 0;

        /// <summary>
        /// 获取聚合根的领域事件
        /// </summary>
        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        /// <summary>
        /// 添加领域事件
        /// </summary>
        /// <param name="domainEvent">领域事件</param>
        protected void AddDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        /// <summary>
        /// 应用事件 - 用于事件溯源
        /// </summary>
        /// <param name="event">领域事件</param>
        public abstract void ApplyEvent(DomainEvent @event);

        /// <summary>
        /// 应用事件 - 用于事件溯源
        /// </summary>
        /// <param name="events">领域事件列表</param>
        public void ApplyEvents(IEnumerable<DomainEvent> events)
        {
            foreach (var @event in events)
            {
                ApplyEvent(@event);
            }
        }

        /// <summary>
        /// 增加版本号
        /// </summary>
        /// <param name="increment">增量</param>
        public void IncrementVersion(int increment)
        {
            if (increment <= 0)
            {
                throw new ArgumentException("版本增量必须大于0", nameof(increment));
            }
            
            Version += increment;
        }

        /// <summary>
        /// 移除领域事件
        /// </summary>
        /// <param name="domainEvent">领域事件</param>
        public void RemoveDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Remove(domainEvent);
        }

        /// <summary>
        /// 清除领域事件
        /// </summary>
        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
} 