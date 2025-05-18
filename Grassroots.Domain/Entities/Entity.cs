using System.Collections.Generic;
using Grassroots.Domain.Events;

namespace Grassroots.Domain.Entities;

/// <summary>
/// 实体基类
/// </summary>
/// <typeparam name="TKey">实体ID类型</typeparam>
public abstract class Entity<TKey> : IHasDomainEvents
{
    private List<IDomainEvent> _domainEvents;
    
    /// <summary>
    /// 实体ID
    /// </summary>
    public TKey Id { get; protected set; }
    
    /// <summary>
    /// 领域事件集合
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents?.AsReadOnly();
    
    /// <summary>
    /// 添加领域事件
    /// </summary>
    /// <param name="domainEvent">领域事件</param>
    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents ??= new List<IDomainEvent>();
        _domainEvents.Add(domainEvent);
    }
    
    /// <summary>
    /// 删除领域事件
    /// </summary>
    /// <param name="domainEvent">领域事件</param>
    public void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents?.Remove(domainEvent);
    }
    
    /// <summary>
    /// 清除所有领域事件
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents?.Clear();
    }
    
    /// <summary>
    /// 相等性比较
    /// </summary>
    /// <param name="obj">要比较的对象</param>
    /// <returns>是否相等</returns>
    public override bool Equals(object obj)
    {
        var entity = obj as Entity<TKey>;
        if (entity == null)
            return false;
            
        if (Object.ReferenceEquals(this, entity))
            return true;
            
        if (GetType() != entity.GetType())
            return false;
            
        if (EqualityComparer<TKey>.Default.Equals(Id, default) || 
            EqualityComparer<TKey>.Default.Equals(entity.Id, default))
            return false;
            
        return EqualityComparer<TKey>.Default.Equals(Id, entity.Id);
    }
    
    /// <summary>
    /// 获取哈希码
    /// </summary>
    /// <returns>哈希码</returns>
    public override int GetHashCode()
    {
        if (EqualityComparer<TKey>.Default.Equals(Id, default))
            return base.GetHashCode();
            
        return Id.GetHashCode() ^ 31;
    }
    
    /// <summary>
    /// 相等运算符
    /// </summary>
    public static bool operator ==(Entity<TKey> left, Entity<TKey> right)
    {
        if (Object.ReferenceEquals(left, null))
            return Object.ReferenceEquals(right, null);
            
        return left.Equals(right);
    }
    
    /// <summary>
    /// 不等运算符
    /// </summary>
    public static bool operator !=(Entity<TKey> left, Entity<TKey> right)
    {
        return !(left == right);
    }
} 