using System;
using Grassroots.Domain.Events;

namespace Grassroots.Domain.Entities;

/// <summary>
/// 聚合根基类
/// </summary>
/// <typeparam name="TKey">聚合根ID类型</typeparam>
public abstract class AggregateRoot<TKey> : Entity<TKey>
{
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedOn { get; protected set; }
    
    /// <summary>
    /// 最后修改时间
    /// </summary>
    public DateTime? LastModifiedOn { get; protected set; }
    
    /// <summary>
    /// 设置创建信息
    /// </summary>
    public virtual void SetCreated()
    {
        CreatedOn = DateTime.UtcNow;
    }
    
    /// <summary>
    /// 设置更新信息
    /// </summary>
    public virtual void SetModified()
    {
        LastModifiedOn = DateTime.UtcNow;
    }
} 