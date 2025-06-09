using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Grassroots.Domain.Common;

/// <summary>
/// 实体基类，所有领域实体的基础类
/// 提供实体标识和相等性比较功能
/// 遵循DDD实体模式，实体由其唯一标识来区分
/// </summary>
/// <typeparam name="TKey">实体ID类型，可以是int、long、Guid等</typeparam>
public abstract class BaseEntity<TKey>
    where TKey : notnull, IEquatable<TKey>
{
    private bool _isTransient = true;
    private TKey? _id;

    /// <summary>
    /// 实体的唯一标识符
    /// 在DDD中，实体的相等性由ID决定，而不是其属性
    /// </summary>
    /// <exception cref="InvalidOperationException">当尝试访问未设置的ID时抛出</exception>
    public TKey Id
    {
        get
        {
            if (_id is null)
            {
                throw new InvalidOperationException(
                    "实体ID尚未设置。在DDD中，实体必须有一个唯一标识。请确保在创建实体时设置ID，或使用工厂方法创建实体。");
            }
            return _id;
        }
        init
        {
            ArgumentNullException.ThrowIfNull(value);
            _id = value;
            _isTransient = false;
        }
    }

    /// <summary>
    /// 检查实体是否是临时的（尚未持久化）
    /// </summary>
    public bool IsTransient => _isTransient;

    /// <summary>
    /// 设置实体ID
    /// </summary>
    /// <param name="id">实体ID</param>
    /// <exception cref="ArgumentNullException">当id为null时抛出</exception>
    /// <exception cref="InvalidOperationException">当尝试重复设置ID时抛出</exception>
    protected void SetId(TKey id)
    {
        ArgumentNullException.ThrowIfNull(id);
        
        if (_id is not null)
        {
            throw new InvalidOperationException(
                "实体ID已经设置。在DDD中，实体ID一旦设置就不能更改。如果需要更改ID，请创建新的实体。");
        }

        _id = id;
        _isTransient = false;
    }

    protected BaseEntity()
    {
        _isTransient = true;
    }

    /// <summary>
    /// 重写Equals方法，实现基于ID的实体相等性比较
    /// 在DDD中，两个具有相同ID的实体被视为同一实体，即使它们的其他属性不同
    /// </summary>
    /// <param name="obj">要比较的对象</param>
    /// <returns>如果对象是同一类型且ID相同则返回true，否则返回false</returns>
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is null)
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        if (obj is not BaseEntity<TKey> other)
            return false;

        if (GetType() != other.GetType())
            return false;

        if (IsTransient || other.IsTransient)
            return false;

        return Id.Equals(other.Id);
    }

    /// <summary>
    /// 重写GetHashCode方法，确保具有相同ID的实体具有相同的哈希码
    /// 这对于在字典和哈希集合中正确存储实体很重要
    /// </summary>
    /// <returns>基于实体ID的哈希码</returns>
    public override int GetHashCode()
    {
        if (IsTransient)
            return base.GetHashCode();

        return $"{GetType().Name}_{Id}".GetHashCode();
    }

    /// <summary>
    /// 重载相等运算符，提供语法糖以简化实体比较
    /// 允许使用 entity1 == entity2 语法进行比较
    /// </summary>
    public static bool operator ==(BaseEntity<TKey>? left, BaseEntity<TKey>? right)
    {
        if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
            return true;

        if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            return false;

        return left.Equals(right);
    }

    /// <summary>
    /// 重载不等运算符，提供语法糖以简化实体比较
    /// 允许使用 entity1 != entity2 语法进行比较
    /// </summary>
    public static bool operator !=(BaseEntity<TKey>? left, BaseEntity<TKey>? right)
    {
        return !(left == right);
    }
}


