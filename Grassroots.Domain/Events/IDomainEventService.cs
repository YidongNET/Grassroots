namespace Grassroots.Domain.Events;

/// <summary>
/// 领域事件服务接口
/// </summary>
public interface IDomainEventService
{
    /// <summary>
    /// 发布领域事件
    /// </summary>
    /// <param name="domainEvent">领域事件</param>
    Task PublishAsync(IDomainEvent domainEvent);
    
    /// <summary>
    /// 发布多个领域事件
    /// </summary>
    /// <param name="domainEvents">领域事件集合</param>
    Task PublishAsync(IEnumerable<IDomainEvent> domainEvents);
} 