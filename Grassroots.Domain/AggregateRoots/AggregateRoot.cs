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