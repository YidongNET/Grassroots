using System.Collections.Generic;
using Grassroots.Domain.Events;

namespace Grassroots.Domain.Entities;

/// <summary>
/// 支持事件溯源的聚合根基类
/// 事件溯源是一种设计模式，通过存储状态变更事件而非最终状态来持久化实体
/// 可以通过重放事件历史来重建实体的状态，支持时间点查询和完整审计
/// 这种模式有助于捕获业务操作的全过程，而不仅仅是结果
/// </summary>
/// <typeparam name="TKey">聚合根ID类型，可以是int、long、Guid等</typeparam>
public abstract class EventSourcedAggregateRoot<TKey> : AggregateRoot<TKey>
{
    private readonly List<IDomainEvent> _uncommittedEvents = new List<IDomainEvent>();
    
    /// <summary>
    /// 当前版本号
    /// 每应用一个事件，版本号加1，用于乐观并发控制
    /// 确保多个并发操作不会意外覆盖彼此的更改
    /// </summary>
    public int Version { get; protected set; }
    
    /// <summary>
    /// 未提交事件列表
    /// 包含自上次持久化以来应用于聚合根的所有事件
    /// 这些事件将被持久化到事件存储中
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> UncommittedEvents => _uncommittedEvents.AsReadOnly();
    
    /// <summary>
    /// 应用事件到聚合根
    /// 这是事件溯源的核心方法，改变聚合根状态并记录事件
    /// 子类中的状态变更应通过此方法实现
    /// </summary>
    /// <param name="event">要应用的领域事件</param>
    protected void ApplyEvent(IDomainEvent @event)
    {
        // 调用子类实现的Apply方法
        InvokeApply(@event);
        // 递增版本号，确保事件按序应用
        Version++;
        // 添加到未提交事件列表，等待持久化
        _uncommittedEvents.Add(@event);
    }
    
    /// <summary>
    /// 调用Apply方法
    /// 通过反射机制找到并调用对应事件类型的Apply方法
    /// 子类需要为每种事件类型实现一个Apply方法
    /// </summary>
    /// <param name="event">领域事件实例</param>
    private void InvokeApply(IDomainEvent @event)
    {
        var method = GetType().GetMethod("Apply", new[] { @event.GetType() });
        if (method == null)
        {
            throw new InvalidOperationException($"The Apply method for {@event.GetType().Name} was not found in the aggregate {GetType().Name}");
        }
        method.Invoke(this, new object[] { @event });
    }
    
    /// <summary>
    /// 从历史事件重建聚合根状态
    /// 按照事件发生的时间顺序重放所有事件
    /// 用于从事件存储中恢复聚合根
    /// </summary>
    /// <param name="events">按时间顺序排列的历史事件列表</param>
    public void LoadFromHistory(IEnumerable<IDomainEvent> events)
    {
        foreach (var @event in events)
        {
            // 应用事件但不添加到未提交事件列表
            // 因为这些事件已经被持久化过了
            InvokeApply(@event);
            Version++;
        }
    }
    
    /// <summary>
    /// 标记事件已提交
    /// 在事件被成功持久化到事件存储后调用
    /// 清空未提交事件列表，避免重复持久化
    /// </summary>
    public void MarkEventsAsCommitted()
    {
        _uncommittedEvents.Clear();
    }
} 