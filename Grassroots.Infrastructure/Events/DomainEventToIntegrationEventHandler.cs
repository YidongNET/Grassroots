using Grassroots.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Grassroots.Infrastructure.Events;

/// <summary>
/// 领域事件到集成事件的处理器
/// </summary>
/// <typeparam name="TDomainEvent">领域事件类型</typeparam>
public class DomainEventToIntegrationEventHandler<TDomainEvent> : IDomainEventHandler<TDomainEvent>
    where TDomainEvent : IDomainEvent
{
    private readonly IDomainToIntegrationEventMapper _mapper;
    private readonly IEventBus _eventBus;
    private readonly ILogger<DomainEventToIntegrationEventHandler<TDomainEvent>> _logger;
    
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="mapper">事件映射器</param>
    /// <param name="eventBus">事件总线</param>
    /// <param name="logger">日志</param>
    public DomainEventToIntegrationEventHandler(
        IDomainToIntegrationEventMapper mapper,
        IEventBus eventBus,
        ILogger<DomainEventToIntegrationEventHandler<TDomainEvent>> logger)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    /// <summary>
    /// 处理领域事件
    /// </summary>
    /// <param name="notification">领域事件</param>
    /// <param name="cancellationToken">取消标记</param>
    public async Task Handle(TDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling domain event {DomainEventType}", typeof(TDomainEvent).Name);
        
        var integrationEvent = _mapper.MapToIntegrationEvent(notification);
        if (integrationEvent != null)
        {
            _logger.LogInformation("Publishing integration event {IntegrationEventType} from domain event {DomainEventType}",
                integrationEvent.EventTypeName, typeof(TDomainEvent).Name);
            await _eventBus.PublishAsync(integrationEvent);
        }
    }
} 