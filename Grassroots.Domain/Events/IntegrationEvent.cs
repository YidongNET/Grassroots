using System.Text.Json.Serialization;

namespace Grassroots.Domain.Events;

/// <summary>
/// 集成事件基类
/// </summary>
public class IntegrationEvent : IIntegrationEvent
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public IntegrationEvent()
    {
        Id = Guid.NewGuid();
        CreationDate = DateTime.UtcNow;
        EventTypeName = GetType().Name;
    }

    /// <summary>
    /// 带参数的构造函数
    /// </summary>
    /// <param name="id">事件ID</param>
    /// <param name="creationDate">创建时间</param>
    [JsonConstructor]
    public IntegrationEvent(Guid id, DateTime creationDate)
    {
        Id = id;
        CreationDate = creationDate;
        EventTypeName = GetType().Name;
    }

    /// <summary>
    /// 事件ID
    /// </summary>
    [JsonInclude]
    public Guid Id { get; private set; }

    /// <summary>
    /// 事件创建时间
    /// </summary>
    [JsonInclude]
    public DateTime CreationDate { get; private set; }
    
    /// <summary>
    /// 事件类型名称
    /// </summary>
    [JsonInclude]
    public string EventTypeName { get; private set; }
} 