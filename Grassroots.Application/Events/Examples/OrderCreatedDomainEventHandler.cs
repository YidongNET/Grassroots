using System;
using System.Threading;
using System.Threading.Tasks;
using Grassroots.Domain.Events;
using Grassroots.Domain.Events.Examples;
using Microsoft.Extensions.Logging;

namespace Grassroots.Application.Events.Examples
{
    /// <summary>
    /// 订单创建领域事件处理器示例
    /// </summary>
    public class OrderCreatedDomainEventHandler : IDomainEventHandler<OrderCreatedEvent>
    {
        private readonly ILogger<OrderCreatedDomainEventHandler> _logger;
        private readonly IIntegrationEventBus _integrationEventBus;

        public OrderCreatedDomainEventHandler(
            ILogger<OrderCreatedDomainEventHandler> logger,
            IIntegrationEventBus integrationEventBus)
        {
            _logger = logger;
            _integrationEventBus = integrationEventBus;
        }

        /// <summary>
        /// 处理订单创建领域事件
        /// </summary>
        /// <param name="event">订单创建事件</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>任务</returns>
        public async Task HandleAsync(OrderCreatedEvent @event, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"订单已创建: {@event.OrderId}, 客户: {@event.CustomerId}, 金额: {@event.Amount}");

            // 在这里执行领域事件的处理逻辑
            // 例如：更新库存、计算统计数据等

            // 发布集成事件
            var integrationEvent = new OrderCreatedIntegrationEvent(
                @event.OrderId,
                @event.CustomerId,
                @event.Amount,
                @event.ItemCount);

            await _integrationEventBus.PublishAsync(integrationEvent, cancellationToken);
        }
    }
} 