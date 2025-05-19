using System;
using Grassroots.Domain.Events;

namespace Grassroots.Domain.Entities;

/// <summary>
/// 聚合根基类
/// 在DDD中，聚合根是一个实体，同时也是聚合的入口点，负责维护聚合的一致性边界
/// 聚合根是一组相关对象的集合，作为一个整体来修改和持久化
/// 外部只能引用聚合根，而不能直接引用聚合内部的实体
/// </summary>
/// <typeparam name="TKey">聚合根ID类型，可以是int、long、Guid等</typeparam>
public abstract class AggregateRoot<TKey> : Entity<TKey>
{
    /// <summary>
    /// 聚合根的创建时间
    /// 用于跟踪实体的生命周期，支持审计功能
    /// </summary>
    public DateTime CreatedOn { get; protected set; }
    
    /// <summary>
    /// 聚合根的最后修改时间
    /// 用于跟踪实体的变更历史，支持审计功能
    /// 可为空，表示实体自创建后未被修改过
    /// </summary>
    public DateTime? LastModifiedOn { get; protected set; }
    
    /// <summary>
    /// 设置创建信息
    /// 在实体首次创建时调用此方法，记录创建时间
    /// 通常在仓储的Add方法中或领域服务创建实体时调用
    /// </summary>
    public virtual void SetCreated()
    {
        CreatedOn = DateTime.UtcNow;
    }
    
    /// <summary>
    /// 设置更新信息
    /// 在实体被更新时调用此方法，记录最后修改时间
    /// 通常在仓储的Update方法中或领域服务更新实体时调用
    /// </summary>
    public virtual void SetModified()
    {
        LastModifiedOn = DateTime.UtcNow;
    }
} 