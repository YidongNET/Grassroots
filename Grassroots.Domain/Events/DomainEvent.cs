namespace Grassroots.Domain.Events;

/// <summary>
/// 领域事件基类
/// </summary>
public abstract class DomainEvent : IDomainEvent
{
    /// <summary>
    /// 构造函数
    /// </summary>
    protected DomainEvent()
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        EventType = GetType().Name;
    }

    /// <summary>
    /// 事件ID
    /// </summary>
    public Guid EventId { get; private set; }

    /// <summary>
    /// 事件发生时间
    /// </summary>
    public DateTime OccurredOn { get; private set; }

    /// <summary>
    /// 事件类型名称
    /// </summary>
    public string EventType { get; private set; }
} 