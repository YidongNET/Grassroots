using Grassroots.Domain.Events;
using MediatR;

namespace Grassroots.Application.Events;

/// <summary>
/// 领域事件通知适配器
/// 将领域事件包装为 MediatR 通知
/// </summary>
/// <typeparam name="TDomainEvent">领域事件类型</typeparam>
public class DomainEventNotification<TDomainEvent> : INotification
    where TDomainEvent : IDomainEvent
{
    /// <summary>
    /// 领域事件实例
    /// </summary>
    public TDomainEvent DomainEvent { get; }

    public DomainEventNotification(TDomainEvent domainEvent)
    {
        DomainEvent = domainEvent;
    }
} 