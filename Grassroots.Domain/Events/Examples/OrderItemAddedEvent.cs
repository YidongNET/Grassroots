using System;

namespace Grassroots.Domain.Events.Examples
{
    /// <summary>
    /// 订单项添加事件
    /// </summary>
    public class OrderItemAddedEvent : DomainEvent
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public Guid OrderId { get; }

        /// <summary>
        /// 订单项ID
        /// </summary>
        public Guid OrderItemId { get; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public Guid ProductId { get; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; }

        /// <summary>
        /// 订单项添加事件
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <param name="orderItemId">订单项ID</param>
        /// <param name="productId">产品ID</param>
        /// <param name="quantity">数量</param>
        /// <param name="unitPrice">单价</param>
        public OrderItemAddedEvent(Guid orderId, Guid orderItemId, Guid productId, int quantity, decimal unitPrice)
            : base(orderId.ToString(), "Order", 1)
        {
            OrderId = orderId;
            OrderItemId = orderItemId;
            ProductId = productId;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }
    }
} 