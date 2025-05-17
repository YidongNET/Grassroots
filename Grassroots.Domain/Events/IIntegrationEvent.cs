using System;

namespace Grassroots.Domain.Events
{
    /// <summary>
    /// 集成事件接口
    /// </summary>
    public interface IIntegrationEvent
    {
        /// <summary>
        /// 事件唯一标识
        /// </summary>
        Guid Id { get; }
        
        /// <summary>
        /// 事件创建时间
        /// </summary>
        DateTime CreationDate { get; }
        
        /// <summary>
        /// 事件类型
        /// </summary>
        string EventType { get; }
    }
} 