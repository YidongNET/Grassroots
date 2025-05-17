using System.Collections.Generic;

namespace Grassroots.Domain.Events
{
    /// <summary>
    /// 事件溯源聚合接口
    /// </summary>
    public interface IEventSourcedAggregate
    {
        /// <summary>
        /// 聚合ID
        /// </summary>
        string Id { get; }
        
        /// <summary>
        /// 聚合版本
        /// </summary>
        int Version { get; }
        
        /// <summary>
        /// 获取未提交的事件
        /// </summary>
        IEnumerable<IDomainEvent> GetUncommittedEvents();
        
        /// <summary>
        /// 应用事件到聚合
        /// </summary>
        /// <param name="event">领域事件</param>
        void Apply(IDomainEvent @event);
        
        /// <summary>
        /// 标记事件已提交
        /// </summary>
        void MarkEventsAsCommitted();
    }
} 