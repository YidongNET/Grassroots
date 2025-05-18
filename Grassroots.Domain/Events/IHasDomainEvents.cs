using System.Collections.Generic;

namespace Grassroots.Domain.Events;

/// <summary>
/// 具有领域事件的实体接口
/// </summary>
public interface IHasDomainEvents
{
    /// <summary>
    /// 获取实体的领域事件
    /// </summary>
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    
    /// <summary>
    /// 添加领域事件
    /// </summary>
    /// <param name="domainEvent">领域事件</param>
    void AddDomainEvent(IDomainEvent domainEvent);
    
    /// <summary>
    /// 删除领域事件
    /// </summary>
    /// <param name="domainEvent">领域事件</param>
    void RemoveDomainEvent(IDomainEvent domainEvent);
    
    /// <summary>
    /// 清除所有领域事件
    /// </summary>
    void ClearDomainEvents();
} 