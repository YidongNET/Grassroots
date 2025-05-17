using Grassroots.Domain.Events;
using System.Threading.Tasks;

namespace Grassroots.Application.Common.Interfaces
{
    /// <summary>
    /// 集成事件处理器接口
    /// </summary>
    /// <typeparam name="TIntegrationEvent">集成事件类型</typeparam>
    public interface IIntegrationEventHandler<in TIntegrationEvent> 
        where TIntegrationEvent : IIntegrationEvent
    {
        /// <summary>
        /// 处理集成事件
        /// </summary>
        /// <param name="event">集成事件</param>
        /// <returns>任务</returns>
        Task HandleAsync(TIntegrationEvent @event);
    }
} 