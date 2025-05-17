using System;
using System.Text.Json.Serialization;

namespace Grassroots.Domain.Events
{
    /// <summary>
    /// 集成事件基类
    /// </summary>
    public class IntegrationEvent : IIntegrationEvent
    {
        /// <summary>
        /// 事件唯一标识
        /// </summary>
        [JsonInclude]
        public Guid Id { get; private set; }
        
        /// <summary>
        /// 事件创建时间
        /// </summary>
        [JsonInclude]
        public DateTime CreationDate { get; private set; }
        
        /// <summary>
        /// 事件类型
        /// </summary>
        [JsonIgnore]
        public string EventType => GetType().Name;

        public IntegrationEvent()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }

        [JsonConstructor]
        public IntegrationEvent(Guid id, DateTime creationDate)
        {
            Id = id;
            CreationDate = creationDate;
        }
    }
} 