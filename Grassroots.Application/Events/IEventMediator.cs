using System.Threading;
using System.Threading.Tasks;
using Grassroots.Domain.AggregateRoots;

namespace Grassroots.Application.Events
{
    /// <summary>
    /// 事件中介者接口，负责协调领域事件的处理和发布
    /// </summary>
    public interface IEventMediator
    {
        /// <summary>
        /// 发布聚合根中的所有领域事件
        /// </summary>
        /// <param name="aggregateRoot">聚合根</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>任务</returns>
        Task PublishEventsAsync(AggregateRoot aggregateRoot, CancellationToken cancellationToken = default);

        /// <summary>
        /// 从事件存储中加载聚合根
        /// </summary>
        /// <typeparam name="T">聚合根类型</typeparam>
        /// <param name="aggregateId">聚合根ID</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>聚合根</returns>
        Task<T> LoadAggregateAsync<T>(string aggregateId, CancellationToken cancellationToken = default) where T : AggregateRoot, new();
    }
} 