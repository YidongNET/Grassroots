namespace Grassroots.Domain.Events;

/// <summary>
/// 领域事件发布服务接口
/// </summary>
public interface IDomainEventPublisher
{
    /// <summary>
    /// 发布领域事件
    /// </summary>
    /// <param name="event">要发布的领域事件</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task PublishAsync(IDomainEvent @event, CancellationToken cancellationToken = default);

    /// <summary>
    /// 发布多个领域事件
    /// </summary>
    /// <param name="events">要发布的领域事件集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task PublishAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken = default);
} 