using Grassroots.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Grassroots.Infrastructure.Events;

/// <summary>
/// 领域事件服务实现
/// </summary>
public class DomainEventService : IDomainEventService
{
    private readonly IMediator _mediator;
    private readonly ILogger<DomainEventService> _logger;
    
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="mediator">MediatR中介者</param>
    /// <param name="logger">日志</param>
    public DomainEventService(IMediator mediator, ILogger<DomainEventService> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    /// <summary>
    /// 发布领域事件
    /// </summary>
    /// <param name="domainEvent">领域事件</param>
    public async Task PublishAsync(IDomainEvent domainEvent)
    {
        _logger.LogInformation("Publishing domain event. Event: {event}", domainEvent.EventType);
        await _mediator.Publish(domainEvent);
    }
    
    /// <summary>
    /// 发布多个领域事件
    /// </summary>
    /// <param name="domainEvents">领域事件集合</param>
    public async Task PublishAsync(IEnumerable<IDomainEvent> domainEvents)
    {
        var events = domainEvents.ToList();
        _logger.LogInformation("Publishing {count} domain events", events.Count);
        
        foreach (var domainEvent in events)
        {
            await PublishAsync(domainEvent);
        }
    }
} 