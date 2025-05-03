using System.Threading;
using System.Threading.Tasks;

namespace Grassroots.Domain.Events
{
    /// <summary>
    /// 领域事件总线接口
    /// </summary>
    public interface IDomainEventBus
    {
        /// <summary>
        /// 发布事件
        /// </summary>
        /// <param name="event">事件</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>任务</returns>
        Task PublishAsync(DomainEvent @event, CancellationToken cancellationToken = default);

        /// <summary>
        /// 注册事件处理器
        /// </summary>
        /// <typeparam name="TEvent">事件类型</typeparam>
        /// <typeparam name="THandler">处理器类型</typeparam>
        void Subscribe<TEvent, THandler>()
            where TEvent : DomainEvent
            where THandler : IDomainEventHandler<TEvent>;
    }
} 