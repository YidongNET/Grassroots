using System.Threading;
using System.Threading.Tasks;
using Grassroots.Domain.Events;
using Grassroots.Domain.Events.Examples;
using Microsoft.Extensions.Logging;

namespace Grassroots.Application.Events.Examples
{
    /// <summary>
    /// 订单创建集成事件处理器示例
    /// </summary>
    public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
    {
        private readonly ILogger<OrderCreatedIntegrationEventHandler> _logger;

        public OrderCreatedIntegrationEventHandler(ILogger<OrderCreatedIntegrationEventHandler> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 处理订单创建集成事件
        /// </summary>
        /// <param name="event">订单创建集成事件</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>任务</returns>
        public Task HandleAsync(OrderCreatedIntegrationEvent @event, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"处理集成事件: 订单已创建 {@event.OrderId}, 客户: {@event.CustomerId}, 金额: {@event.Amount}");

            // 在这里执行集成事件的处理逻辑
            // 例如：发送通知、更新其他服务的数据等

            return Task.CompletedTask;
        }
    }
} 