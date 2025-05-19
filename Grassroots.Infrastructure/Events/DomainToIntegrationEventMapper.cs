using System.Collections.Concurrent;
using Grassroots.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Grassroots.Infrastructure.Events;

/// <summary>
/// 领域事件到集成事件的映射接口
/// </summary>
public interface IDomainToIntegrationEventMapper
{
    /// <summary>
    /// 注册事件映射
    /// </summary>
    /// <typeparam name="TDomainEvent">领域事件类型</typeparam>
    /// <typeparam name="TIntegrationEvent">集成事件类型</typeparam>
    /// <param name="mapper">映射函数</param>
    void RegisterMapping<TDomainEvent, TIntegrationEvent>(Func<TDomainEvent, TIntegrationEvent> mapper)
        where TDomainEvent : IDomainEvent
        where TIntegrationEvent : IIntegrationEvent;
    
    /// <summary>
    /// 将领域事件映射为集成事件
    /// </summary>
    /// <param name="domainEvent">领域事件</param>
    /// <returns>集成事件，如果没有对应的映射则返回null</returns>
    IIntegrationEvent MapToIntegrationEvent(IDomainEvent domainEvent);
}

/// <summary>
/// 领域事件到集成事件的映射实现
/// </summary>
public class DomainToIntegrationEventMapper : IDomainToIntegrationEventMapper
{
    private readonly ConcurrentDictionary<Type, Func<IDomainEvent, IIntegrationEvent>> _mappers = new();
    private readonly ILogger<DomainToIntegrationEventMapper> _logger;
    
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志</param>
    public DomainToIntegrationEventMapper(ILogger<DomainToIntegrationEventMapper> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    /// <summary>
    /// 注册事件映射
    /// </summary>
    /// <typeparam name="TDomainEvent">领域事件类型</typeparam>
    /// <typeparam name="TIntegrationEvent">集成事件类型</typeparam>
    /// <param name="mapper">映射函数</param>
    public void RegisterMapping<TDomainEvent, TIntegrationEvent>(Func<TDomainEvent, TIntegrationEvent> mapper)
        where TDomainEvent : IDomainEvent
        where TIntegrationEvent : IIntegrationEvent
    {
        var domainEventType = typeof(TDomainEvent);
        
        if (_mappers.ContainsKey(domainEventType))
        {
            _logger.LogWarning("Mapping for domain event type {DomainEventType} already registered, overwriting", domainEventType.Name);
        }
        
        _logger.LogInformation("Registering mapping from {DomainEventType} to {IntegrationEventType}", 
            typeof(TDomainEvent).Name, typeof(TIntegrationEvent).Name);
        
        _mappers[domainEventType] = e => mapper((TDomainEvent)e);
    }
    
    /// <summary>
    /// 将领域事件映射为集成事件
    /// </summary>
    /// <param name="domainEvent">领域事件</param>
    /// <returns>集成事件，如果没有对应的映射则返回null</returns>
    public IIntegrationEvent MapToIntegrationEvent(IDomainEvent domainEvent)
    {
        var domainEventType = domainEvent.GetType();
        
        if (_mappers.TryGetValue(domainEventType, out var mapper))
        {
            _logger.LogDebug("Mapping domain event {DomainEventType} to integration event", domainEventType.Name);
            return mapper(domainEvent);
        }
        
        _logger.LogWarning("No mapping registered for domain event type {DomainEventType}", domainEventType.Name);
        return null;
    }
} 