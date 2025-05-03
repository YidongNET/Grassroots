using System;
using System.Collections.Generic;
using Grassroots.Domain.Events;
using Grassroots.Domain.Events.Examples;

namespace Grassroots.Domain.AggregateRoots.Examples
{
    /// <summary>
    /// 订单聚合根示例
    /// </summary>
    public class Order : AggregateRoot
    {
        /// <summary>
        /// 客户ID
        /// </summary>
        public Guid CustomerId { get; private set; }

        /// <summary>
        /// 订单金额
        /// </summary>
        public decimal Amount { get; private set; }

        /// <summary>
        /// 订单项
        /// </summary>
        public List<OrderItem> Items { get; private set; } = new List<OrderItem>();

        /// <summary>
        /// 订单状态
        /// </summary>
        public OrderStatus Status { get; private set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// 公共无参构造函数，用于事件溯源
        /// </summary>
        public Order()
        {
        }

        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="customerId">客户ID</param>
        /// <returns>订单</returns>
        public static Order Create(Guid customerId)
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                Status = OrderStatus.Created,
                CreatedAt = DateTime.UtcNow
            };

            return order;
        }

        /// <summary>
        /// 添加订单项
        /// </summary>
        /// <param name="productId">产品ID</param>
        /// <param name="quantity">数量</param>
        /// <param name="unitPrice">单价</param>
        public void AddItem(Guid productId, int quantity, decimal unitPrice)
        {
            if (Status != OrderStatus.Created)
            {
                throw new InvalidOperationException("只能在创建状态添加订单项");
            }

            var orderItem = new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                Quantity = quantity,
                UnitPrice = unitPrice
            };

            Items.Add(orderItem);
            CalculateAmount();

            // 添加领域事件
            var @event = new OrderItemAddedEvent(
                Id,
                orderItem.Id,
                productId,
                quantity,
                unitPrice);

            AddDomainEvent(@event);
        }

        /// <summary>
        /// 提交订单
        /// </summary>
        public void Submit()
        {
            if (Status != OrderStatus.Created)
            {
                throw new InvalidOperationException("只能提交处于创建状态的订单");
            }

            if (Items.Count == 0)
            {
                throw new InvalidOperationException("订单必须至少包含一个订单项");
            }

            Status = OrderStatus.Submitted;

            // 添加领域事件
            var @event = new OrderCreatedEvent(
                Id,
                CustomerId,
                Amount,
                Items.Count);

            AddDomainEvent(@event);
        }

        /// <summary>
        /// 应用事件 - 用于事件溯源
        /// </summary>
        /// <param name="event">领域事件</param>
        public override void ApplyEvent(DomainEvent @event)
        {
            switch (@event)
            {
                case OrderCreatedEvent orderCreated:
                    Apply(orderCreated);
                    break;
                case OrderItemAddedEvent orderItemAdded:
                    Apply(orderItemAdded);
                    break;
                default:
                    throw new InvalidOperationException($"未知的事件类型: {@event.GetType().Name}");
            }

            Version = @event.Version;
        }

        private void Apply(OrderCreatedEvent @event)
        {
            Id = @event.OrderId;
            CustomerId = @event.CustomerId;
            Amount = @event.Amount;
            Status = OrderStatus.Submitted;
            CreatedAt = @event.OccurredOn;
        }

        private void Apply(OrderItemAddedEvent @event)
        {
            var orderItem = new OrderItem
            {
                Id = @event.OrderItemId,
                ProductId = @event.ProductId,
                Quantity = @event.Quantity,
                UnitPrice = @event.UnitPrice
            };

            Items.Add(orderItem);
            CalculateAmount();
        }

        private void CalculateAmount()
        {
            Amount = 0;
            foreach (var item in Items)
            {
                Amount += item.UnitPrice * item.Quantity;
            }
        }
    }

    /// <summary>
    /// 订单项
    /// </summary>
    public class OrderItem
    {
        /// <summary>
        /// 订单项ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }
    }

    /// <summary>
    /// 订单状态
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// 已创建
        /// </summary>
        Created = 0,

        /// <summary>
        /// 已提交
        /// </summary>
        Submitted = 1,

        /// <summary>
        /// 已付款
        /// </summary>
        Paid = 2,

        /// <summary>
        /// 已发货
        /// </summary>
        Shipped = 3,

        /// <summary>
        /// 已完成
        /// </summary>
        Completed = 4,

        /// <summary>
        /// 已取消
        /// </summary>
        Cancelled = 5
    }
} 