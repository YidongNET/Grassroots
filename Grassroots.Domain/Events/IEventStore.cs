namespace Grassroots.Domain.Events;

/// <summary>
/// 事件存储接口
/// </summary>
public interface IEventStore
{
    /// <summary>
    /// 保存事件
    /// </summary>
    /// <param name="aggregateId">聚合根ID</param>
    /// <param name="events">领域事件集合</param>
    /// <param name="expectedVersion">预期版本号，用于乐观并发控制</param>
    /// <returns>操作是否成功</returns>
    Task<bool> SaveEventsAsync<TKey>(TKey aggregateId, IEnumerable<IDomainEvent> events, int expectedVersion);
    
    /// <summary>
    /// 获取聚合根的事件历史
    /// </summary>
    /// <param name="aggregateId">聚合根ID</param>
    /// <returns>事件历史列表</returns>
    Task<IEnumerable<IDomainEvent>> GetEventsForAggregateAsync<TKey>(TKey aggregateId);
    
    /// <summary>
    /// 获取指定类型的所有事件
    /// </summary>
    /// <typeparam name="T">事件类型</typeparam>
    /// <returns>事件列表</returns>
    Task<IEnumerable<IDomainEvent>> GetEventsOfTypeAsync<T>() where T : IDomainEvent;
} 