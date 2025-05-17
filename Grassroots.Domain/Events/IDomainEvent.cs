using System;
using MediatR;

namespace Grassroots.Domain.Events
{
    /// <summary>
    /// 领域事件接口
    /// </summary>
    public interface IDomainEvent : INotification
    {
        /// <summary>
        /// 事件唯一标识
        /// </summary>
        Guid Id { get; }
        
        /// <summary>
        /// 事件发生时间
        /// </summary>
        DateTime OccurredOn { get; }
        
        /// <summary>
        /// 事件版本
        /// </summary>
        int Version { get; }
    }
} 