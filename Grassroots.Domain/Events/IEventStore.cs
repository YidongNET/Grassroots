using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Grassroots.Domain.Events
{
    /// <summary>
    /// 事件存储接口
    /// </summary>
    public interface IEventStore
    {
        /// <summary>
        /// 保存事件
        /// </summary>
        /// <param name="aggregateId">聚合根ID</param>
        /// <param name="events">事件列表</param>
        /// <param name="expectedVersion">期望版本</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>任务</returns>
        Task SaveEventsAsync(string aggregateId, IEnumerable<DomainEvent> events, int expectedVersion, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取聚合根的事件
        /// </summary>
        /// <param name="aggregateId">聚合根ID</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>事件列表</returns>
        Task<List<DomainEvent>> GetEventsAsync(string aggregateId, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取聚合根的事件
        /// </summary>
        /// <param name="aggregateId">聚合根ID</param>
        /// <param name="fromVersion">起始版本</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>事件列表</returns>
        Task<List<DomainEvent>> GetEventsAsync(string aggregateId, int fromVersion, CancellationToken cancellationToken = default);
    }
} 