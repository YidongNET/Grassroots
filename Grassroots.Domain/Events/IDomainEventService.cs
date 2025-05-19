namespace Grassroots.Domain.Events;

/// <summary>
/// 领域事件服务接口
/// 负责领域事件的分发和处理
/// 作为领域层和基础设施层之间的桥梁，将领域事件转发给对应的处理器
/// 通常在工作单元提交后由基础设施层调用，确保事件处理的一致性和原子性
/// </summary>
public interface IDomainEventService
{
    /// <summary>
    /// 发布领域事件
    /// 将单个领域事件分发给所有注册的处理器
    /// 使用MediatR等中介者模式实现处理器的查找和调用
    /// </summary>
    /// <param name="domainEvent">要发布的领域事件</param>
    /// <returns>异步任务，完成后表示事件已被所有处理器处理</returns>
    Task PublishAsync(IDomainEvent domainEvent);
    
    /// <summary>
    /// 发布多个领域事件
    /// 批量发布多个事件，通常用于处理聚合根中的所有未提交事件
    /// 保证事件按照添加顺序依次处理，维护事件处理的顺序性
    /// </summary>
    /// <param name="domainEvents">要发布的领域事件集合</param>
    /// <returns>异步任务，完成后表示所有事件已被处理</returns>
    Task PublishAsync(IEnumerable<IDomainEvent> domainEvents);
} 