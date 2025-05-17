using System;

namespace Grassroots.Domain.Events
{
    /// <summary>
    /// 领域事件基类
    /// </summary>
    public abstract class DomainEvent : IDomainEvent
    {
        /// <summary>
        /// 事件唯一标识
        /// </summary>
        public Guid Id { get; protected set; }
        
        /// <summary>
        /// 事件发生时间
        /// </summary>
        public DateTime OccurredOn { get; protected set; }
        
        /// <summary>
        /// 事件版本
        /// </summary>
        public int Version { get; protected set; }
        
        /// <summary>
        /// 事件类型名
        /// </summary>
        public string EventType => GetType().Name;

        protected DomainEvent()
        {
            Id = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
            Version = 1;
        }
    }
} 