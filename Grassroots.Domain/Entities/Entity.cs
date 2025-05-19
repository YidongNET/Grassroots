using System.Collections.Generic;
using Grassroots.Domain.Events;

namespace Grassroots.Domain.Entities;

/// <summary>
/// 实体基类，所有领域实体的基础类
/// 提供实体标识、相等性比较和领域事件功能
/// 遵循DDD实体模式，实体由其唯一标识来区分
/// </summary>
/// <typeparam name="TKey">实体ID类型，可以是int、long、Guid等</typeparam>
public abstract class Entity<TKey> : IHasDomainEvents
{
    private List<IDomainEvent> _domainEvents;
    
    /// <summary>
    /// 实体的唯一标识符
    /// 在DDD中，实体的相等性由ID决定，而不是其属性
    /// </summary>
    public TKey Id { get; protected set; }
    
    /// <summary>
    /// 领域事件集合，只读集合确保外部代码不能直接修改事件列表
    /// 通过AddDomainEvent等方法管理事件
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents?.AsReadOnly();
    
    /// <summary>
    /// 添加领域事件到事件集合
    /// 当实体状态发生重要变化时调用此方法
    /// 事件将在UnitOfWork提交时由DomainEventService发布
    /// </summary>
    /// <param name="domainEvent">要添加的领域事件实例</param>
    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents ??= new List<IDomainEvent>();
        _domainEvents.Add(domainEvent);
    }
    
    /// <summary>
    /// 从事件集合中移除指定的领域事件
    /// 如果事件被处理后不再需要，可以调用此方法
    /// </summary>
    /// <param name="domainEvent">要移除的领域事件实例</param>
    public void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents?.Remove(domainEvent);
    }
    
    /// <summary>
    /// 清除所有未处理的领域事件
    /// 通常在事件被发布后由DomainEventService调用
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents?.Clear();
    }
    
    /// <summary>
    /// 重写Equals方法，实现基于ID的实体相等性比较
    /// 在DDD中，两个具有相同ID的实体被视为同一实体，即使它们的其他属性不同
    /// </summary>
    /// <param name="obj">要比较的对象</param>
    /// <returns>如果对象是同一类型且ID相同则返回true，否则返回false</returns>
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
    /// 重写GetHashCode方法，确保具有相同ID的实体具有相同的哈希码
    /// 这对于在字典和哈希集合中正确存储实体很重要
    /// </summary>
    /// <returns>基于实体ID的哈希码</returns>
    public override int GetHashCode()
    {
        if (EqualityComparer<TKey>.Default.Equals(Id, default))
            return base.GetHashCode();
            
        return Id.GetHashCode() ^ 31;
    }
    
    /// <summary>
    /// 重载相等运算符，提供语法糖以简化实体比较
    /// 允许使用 entity1 == entity2 语法进行比较
    /// </summary>
    public static bool operator ==(Entity<TKey> left, Entity<TKey> right)
    {
        if (Object.ReferenceEquals(left, null))
            return Object.ReferenceEquals(right, null);
            
        return left.Equals(right);
    }
    
    /// <summary>
    /// 重载不等运算符，提供语法糖以简化实体比较
    /// 允许使用 entity1 != entity2 语法进行比较
    /// </summary>
    public static bool operator !=(Entity<TKey> left, Entity<TKey> right)
    {
        return !(left == right);
    }
} 