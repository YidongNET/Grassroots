using Grassroots.Domain.Events;
using System.Threading.Tasks;

namespace Grassroots.Application.Common.Interfaces
{
    /// <summary>
    /// 事件总线接口
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// 发布集成事件
        /// </summary>
        /// <param name="event">集成事件</param>
        Task PublishAsync(IIntegrationEvent @event);
        
        /// <summary>
        /// 订阅集成事件
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <typeparam name="TH">事件处理器类型</typeparam>
        void Subscribe<T, TH>()
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>;
        
        /// <summary>
        /// 取消订阅集成事件
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <typeparam name="TH">事件处理器类型</typeparam>
        void Unsubscribe<T, TH>()
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>;
    }
} 