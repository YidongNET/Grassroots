namespace Grassroots.Domain.Events;

/// <summary>
/// 事件总线接口
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// 发布集成事件
    /// </summary>
    /// <param name="event">集成事件</param>
    Task PublishAsync(IIntegrationEvent @event);
    
    /// <summary>
    /// 订阅集成事件
    /// </summary>
    /// <typeparam name="TEvent">事件类型</typeparam>
    /// <typeparam name="THandler">处理器类型</typeparam>
    void Subscribe<TEvent, THandler>()
        where TEvent : IIntegrationEvent
        where THandler : IIntegrationEventHandler<TEvent>;
    
    /// <summary>
    /// 取消订阅集成事件
    /// </summary>
    /// <typeparam name="TEvent">事件类型</typeparam>
    /// <typeparam name="THandler">处理器类型</typeparam>
    void Unsubscribe<TEvent, THandler>()
        where TEvent : IIntegrationEvent
        where THandler : IIntegrationEventHandler<TEvent>;
} 