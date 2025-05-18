namespace Grassroots.Domain.Events;

/// <summary>
/// 集成事件处理器接口
/// </summary>
/// <typeparam name="TIntegrationEvent">集成事件类型</typeparam>
public interface IIntegrationEventHandler<in TIntegrationEvent> 
    where TIntegrationEvent : IIntegrationEvent
{
    /// <summary>
    /// 处理集成事件
    /// </summary>
    /// <param name="event">集成事件</param>
    /// <returns>处理结果</returns>
    Task HandleAsync(TIntegrationEvent @event);
}

/// <summary>
/// 通用集成事件处理器接口
/// </summary>
public interface IIntegrationEventHandler
{
} 