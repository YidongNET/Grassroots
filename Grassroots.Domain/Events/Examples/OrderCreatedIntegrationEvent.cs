using System;

namespace Grassroots.Domain.Events.Examples
{
    /// <summary>
    /// 订单创建集成事件示例
    /// </summary>
    public class OrderCreatedIntegrationEvent : IntegrationEvent
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public Guid OrderId { get; }

        /// <summary>
        /// 客户ID
        /// </summary>
        public Guid CustomerId { get; }

        /// <summary>
        /// 订单金额
        /// </summary>
        public decimal Amount { get; }

        /// <summary>
        /// 订单项数量
        /// </summary>
        public int ItemCount { get; }

        /// <summary>
        /// 订单创建集成事件
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <param name="customerId">客户ID</param>
        /// <param name="amount">订单金额</param>
        /// <param name="itemCount">订单项数量</param>
        public OrderCreatedIntegrationEvent(Guid orderId, Guid customerId, decimal amount, int itemCount)
        {
            OrderId = orderId;
            CustomerId = customerId;
            Amount = amount;
            ItemCount = itemCount;
        }
    }
} 