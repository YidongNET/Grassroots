using System;

namespace Grassroots.Infrastructure.EventSourcing
{
    /// <summary>
    /// 事件描述符
    /// </summary>
    public class EventDescriptor
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();
        
        /// <summary>
        /// 聚合根ID
        /// </summary>
        public string AggregateId { get; set; }
        
        /// <summary>
        /// 事件类型
        /// </summary>
        public string EventType { get; set; }
        
        /// <summary>
        /// 事件数据（序列化后）
        /// </summary>
        public string EventData { get; set; }
        
        /// <summary>
        /// 事件版本
        /// </summary>
        public int Version { get; set; }
        
        /// <summary>
        /// 事件时间戳
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
} 