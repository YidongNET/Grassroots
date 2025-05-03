using System.Threading;
using System.Threading.Tasks;

namespace Grassroots.Domain.Events
{
    /// <summary>
    /// 领域事件处理器接口
    /// </summary>
    /// <typeparam name="TEvent">事件类型</typeparam>
    public interface IDomainEventHandler<in TEvent> where TEvent : DomainEvent
    {
        /// <summary>
        /// 处理事件
        /// </summary>
        /// <param name="event">事件</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>任务</returns>
        Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
    }
} 