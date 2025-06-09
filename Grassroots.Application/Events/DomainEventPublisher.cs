using Grassroots.Domain.Events;
using MediatR;

namespace Grassroots.Application.Events;

/// <summary>
/// 领域事件发布服务实现
/// 使用 MediatR 作为事件发布的基础设施
/// </summary>
public class DomainEventPublisher : IDomainEventPublisher
{
    private readonly IMediator _mediator;

    public DomainEventPublisher(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task PublishAsync(IDomainEvent @event, CancellationToken cancellationToken = default)
    {
        // 创建通用类型的通知类型
        var notificationType = typeof(DomainEventNotification<>).MakeGenericType(@event.GetType());
        // 创建通知实例
        var notification = Activator.CreateInstance(notificationType, @event);
        // 通过 MediatR 发布通知
        await _mediator.Publish(notification, cancellationToken);
    }

    public async Task PublishAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken = default)
    {
        foreach (var @event in events)
        {
            await PublishAsync(@event, cancellationToken);
        }
    }
} 