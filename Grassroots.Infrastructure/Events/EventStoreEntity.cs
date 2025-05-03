using System;

namespace Grassroots.Infrastructure.Events
{
    /// <summary>
    /// 事件存储实体
    /// </summary>
    public class EventStoreEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 聚合根ID
        /// </summary>
        public string AggregateId { get; set; }

        /// <summary>
        /// 聚合根类型
        /// </summary>
        public string AggregateType { get; set; }

        /// <summary>
        /// 事件类型
        /// </summary>
        public string EventType { get; set; }

        /// <summary>
        /// 事件版本
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// 事件数据（JSON序列化）
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// 事件元数据（JSON序列化）
        /// </summary>
        public string Metadata { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
} 