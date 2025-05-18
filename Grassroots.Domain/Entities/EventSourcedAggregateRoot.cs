using System.Collections.Generic;
using Grassroots.Domain.Events;

namespace Grassroots.Domain.Entities;

/// <summary>
/// 支持事件溯源的聚合根基类
/// </summary>
/// <typeparam name="TKey">聚合根ID类型</typeparam>
public abstract class EventSourcedAggregateRoot<TKey> : AggregateRoot<TKey>
{
    private readonly List<IDomainEvent> _uncommittedEvents = new List<IDomainEvent>();
    
    /// <summary>
    /// 当前版本号
    /// </summary>
    public int Version { get; protected set; }
    
    /// <summary>
    /// 未提交事件列表
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> UncommittedEvents => _uncommittedEvents.AsReadOnly();
    
    /// <summary>
    /// 应用事件
    /// </summary>
    /// <param name="event">领域事件</param>
    protected void ApplyEvent(IDomainEvent @event)
    {
        // 调用子类实现的Apply方法
        InvokeApply(@event);
        // 递增版本号
        Version++;
        // 添加到未提交事件列表
        _uncommittedEvents.Add(@event);
    }
    
    /// <summary>
    /// 调用Apply方法
    /// </summary>
    /// <param name="event">领域事件</param>
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
    /// 从历史事件重建聚合根
    /// </summary>
    /// <param name="events">历史事件列表</param>
    public void LoadFromHistory(IEnumerable<IDomainEvent> events)
    {
        foreach (var @event in events)
        {
            // 应用事件但不添加到未提交事件列表
            InvokeApply(@event);
            Version++;
        }
    }
    
    /// <summary>
    /// 标记事件已提交
    /// </summary>
    public void MarkEventsAsCommitted()
    {
        _uncommittedEvents.Clear();
    }
} 