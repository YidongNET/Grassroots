using System;
using System.Threading.Tasks;
using Grassroots.Application.Events;
using Grassroots.Domain.AggregateRoots.Examples;
using Microsoft.AspNetCore.Mvc;

namespace Grassroots.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IEventMediator _eventMediator;

        public OrdersController(IEventMediator eventMediator)
        {
            _eventMediator = eventMediator;
        }

        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="request">请求</param>
        /// <returns>订单ID</returns>
        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
        {
            // 创建订单
            var order = Order.Create(request.CustomerId);

            // 添加订单项
            foreach (var item in request.Items)
            {
                order.AddItem(item.ProductId, item.Quantity, item.UnitPrice);
            }

            // 提交订单
            order.Submit();

            // 发布事件
            await _eventMediator.PublishEventsAsync(order);

            return Ok(new { Id = order.Id });
        }

        /// <summary>
        /// 根据ID获取订单
        /// </summary>
        /// <param name="id">订单ID</param>
        /// <returns>订单</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(Guid id)
        {
            // 从事件存储中加载聚合根
            var order = await _eventMediator.LoadAggregateAsync<Order>(id.ToString());

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }
    }

    /// <summary>
    /// 创建订单请求
    /// </summary>
    public class CreateOrderRequest
    {
        /// <summary>
        /// 客户ID
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// 订单项
        /// </summary>
        public required OrderItemRequest[] Items { get; set; }
    }

    /// <summary>
    /// 订单项请求
    /// </summary>
    public class OrderItemRequest
    {
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
} 