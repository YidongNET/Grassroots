using Grassroots.Domain.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Grassroots.Application.Common.Interfaces
{
    /// <summary>
    /// 事件存储接口
    /// </summary>
    public interface IEventStore
    {
        /// <summary>
        /// 保存事件
        /// </summary>
        /// <param name="aggregateId">聚合ID</param>
        /// <param name="events">事件列表</param>
        /// <param name="expectedVersion">期望版本</param>
        /// <returns>任务</returns>
        Task SaveEventsAsync(string aggregateId, IEnumerable<IDomainEvent> events, int expectedVersion);
        
        /// <summary>
        /// 获取事件
        /// </summary>
        /// <param name="aggregateId">聚合ID</param>
        /// <returns>事件列表</returns>
        Task<List<IDomainEvent>> GetEventsAsync(string aggregateId);
        
        /// <summary>
        /// 获取事件流
        /// </summary>
        /// <param name="aggregateId">聚合ID</param>
        /// <param name="fromVersion">起始版本</param>
        /// <returns>事件列表</returns>
        Task<List<IDomainEvent>> GetEventsAsync(string aggregateId, int fromVersion);
    }
} 