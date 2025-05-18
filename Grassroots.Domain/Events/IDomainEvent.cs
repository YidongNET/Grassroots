using MediatR;

namespace Grassroots.Domain.Events;

/// <summary>
/// 领域事件接口
/// </summary>
public interface IDomainEvent : INotification
{
    /// <summary>
    /// 事件ID
    /// </summary>
    Guid EventId { get; }
    
    /// <summary>
    /// 事件发生时间
    /// </summary>
    DateTime OccurredOn { get; }
    
    /// <summary>
    /// 事件类型名称
    /// </summary>
    string EventType { get; }
} 