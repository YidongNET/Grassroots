using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Grassroots.Domain.Events;

namespace Grassroots.Domain.Common;

/// <summary>
/// 聚合根基类
/// 在DDD中，聚合根是一个实体，同时也是聚合的入口点
/// 负责维护聚合的一致性边界和管理领域事件
/// </summary>
/// <typeparam name="TKey">聚合根标识类型</typeparam>
public abstract class AggregateRoot<TKey> : BaseEntity<TKey>
    where TKey : notnull, IEquatable<TKey>
{
    private readonly List<IDomainEvent> _domainEvents = new();
    private int _version = 1;

    /// <summary>
    /// 聚合版本号，用于乐观并发控制
    /// </summary>
    public int Version => _version;

    /// <summary>
    /// 聚合根的创建时间
    /// </summary>
    public DateTime CreatedOn { get; private set; }
    
    /// <summary>
    /// 聚合根的最后修改时间
    /// </summary>
    public DateTime? LastModifiedOn { get; private set; }

    /// <summary>
    /// 领域事件集合，只读集合确保外部代码不能直接修改事件列表
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="id">聚合根ID</param>
    protected AggregateRoot(TKey id)
    {
        ArgumentNullException.ThrowIfNull(id);
        Id = id;
        CreatedOn = DateTime.UtcNow;
    }

    /// <summary>
    /// 增加聚合版本号
    /// 在聚合根状态发生变化时调用此方法
    /// </summary>
    protected void IncrementVersion()
    {
        _version++;
        LastModifiedOn = DateTime.UtcNow;
    }

    /// <summary>
    /// 添加领域事件到事件集合
    /// 当聚合状态发生重要变化时调用此方法
    /// 事件将在UnitOfWork提交时由DomainEventService发布
    /// </summary>
    /// <param name="eventItem">要添加的领域事件实例</param>
    /// <exception cref="ArgumentNullException">当 eventItem 为 null 时抛出</exception>
    protected void AddDomainEvent(IDomainEvent eventItem)
    {
        ArgumentNullException.ThrowIfNull(eventItem);
        _domainEvents.Add(eventItem);
        IncrementVersion();
    }

    /// <summary>
    /// 从事件集合中移除指定的领域事件
    /// 如果事件被处理后不再需要，可以调用此方法
    /// </summary>
    /// <param name="eventItem">要移除的事件</param>
    /// <exception cref="ArgumentNullException">当 eventItem 为 null 时抛出</exception>
    protected void RemoveDomainEvent(IDomainEvent eventItem)
    {
        ArgumentNullException.ThrowIfNull(eventItem);
        _domainEvents.Remove(eventItem);
    }

    /// <summary>
    /// 清空所有未处理的领域事件
    /// 通常在事件被发布后由DomainEventService调用
    /// </summary>
    internal void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    /// <summary>
    /// 重写 GetHashCode 方法
    /// 在聚合根中，我们使用基类的 GetHashCode 实现，因为它已经包含了基于 ID 的哈希码生成
    /// </summary>
    public override int GetHashCode() => base.GetHashCode();

    /// <summary>
    /// 重写 Equals 方法
    /// 在聚合根中，我们使用基类的 Equals 实现，因为它已经包含了基于 ID 的相等性比较
    /// </summary>
    public override bool Equals([NotNullWhen(true)] object? obj) => base.Equals(obj);
}




