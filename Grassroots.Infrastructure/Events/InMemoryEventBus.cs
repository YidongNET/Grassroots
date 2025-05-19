using System.Collections.Concurrent;
using Grassroots.Domain.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Grassroots.Infrastructure.Events;

/// <summary>
/// 内存版事件总线实现
/// </summary>
public class InMemoryEventBus : IEventBus
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InMemoryEventBus> _logger;
    
    // 存储事件类型与处理器类型的映射关系
    private readonly ConcurrentDictionary<string, List<Type>> _handlers = new();
    
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    /// <param name="logger">日志</param>
    public InMemoryEventBus(IServiceProvider serviceProvider, ILogger<InMemoryEventBus> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    /// <summary>
    /// 发布集成事件
    /// </summary>
    /// <param name="event">集成事件</param>
    public async Task PublishAsync(IIntegrationEvent @event)
    {
        var eventType = @event.GetType().Name;
        _logger.LogInformation("Publishing integration event: {EventName}", eventType);
        
        if (_handlers.TryGetValue(eventType, out var handlers))
        {
            using var scope = _serviceProvider.CreateScope();
            
            foreach (var handlerType in handlers)
            {
                var handler = scope.ServiceProvider.GetRequiredService(handlerType);
                var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(@event.GetType());
                var handleMethod = concreteType.GetMethod("HandleAsync");
                
                try
                {
                    await (Task)handleMethod.Invoke(handler, new object[] { @event });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error handling integration event: {EventName}", eventType);
                }
            }
        }
        else
        {
            _logger.LogWarning("No handlers registered for integration event: {EventName}", eventType);
        }
    }
    
    /// <summary>
    /// 订阅集成事件
    /// </summary>
    /// <typeparam name="TEvent">事件类型</typeparam>
    /// <typeparam name="THandler">处理器类型</typeparam>
    public void Subscribe<TEvent, THandler>() 
        where TEvent : IIntegrationEvent 
        where THandler : IIntegrationEventHandler<TEvent>
    {
        var eventType = typeof(TEvent).Name;
        var handlerType = typeof(THandler);
        
        _logger.LogInformation("Subscribing to event {EventName} with handler {HandlerName}", eventType, handlerType.Name);
        
        if (!_handlers.ContainsKey(eventType))
        {
            _handlers[eventType] = new List<Type>();
        }
        
        if (_handlers[eventType].Contains(handlerType))
        {
            _logger.LogWarning("Handler {HandlerName} already registered for event {EventName}", handlerType.Name, eventType);
            return;
        }
        
        _handlers[eventType].Add(handlerType);
    }
    
    /// <summary>
    /// 取消订阅集成事件
    /// </summary>
    /// <typeparam name="TEvent">事件类型</typeparam>
    /// <typeparam name="THandler">处理器类型</typeparam>
    public void Unsubscribe<TEvent, THandler>() 
        where TEvent : IIntegrationEvent 
        where THandler : IIntegrationEventHandler<TEvent>
    {
        var eventType = typeof(TEvent).Name;
        var handlerType = typeof(THandler);
        
        _logger.LogInformation("Unsubscribing from event {EventName} with handler {HandlerName}", eventType, handlerType.Name);
        
        if (!_handlers.ContainsKey(eventType))
        {
            _logger.LogWarning("No handlers registered for event {EventName}", eventType);
            return;
        }
        
        _handlers[eventType].Remove(handlerType);
        
        if (!_handlers[eventType].Any())
        {
            _handlers.TryRemove(eventType, out _);
        }
    }
} 