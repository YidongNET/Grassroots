using Grassroots.Application.Common.Interfaces;
using Grassroots.Domain.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Grassroots.Infrastructure.EventBus
{
    /// <summary>
    /// 内存实现的事件总线
    /// 用于系统内部的集成事件发布与订阅
    /// 实现了发布/订阅(Pub/Sub)模式，支持跨模块和服务间的松耦合通信
    /// 此实现基于内存，适用于单体应用或作为分布式消息系统的本地实现
    /// </summary>
    public class InMemoryEventBus : IEventBus
    {
        private readonly IEventBusSubscriptionsManager _subsManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<InMemoryEventBus> _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="subsManager">订阅管理器，负责跟踪和管理事件订阅</param>
        /// <param name="serviceProvider">服务提供程序，用于解析事件处理器</param>
        /// <param name="logger">日志记录器</param>
        public InMemoryEventBus(
            IEventBusSubscriptionsManager subsManager,
            IServiceProvider serviceProvider,
            ILogger<InMemoryEventBus> logger)
        {
            _subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// 发布集成事件
        /// 将事件广播给所有订阅该事件类型的处理器
        /// 使用依赖注入容器创建事件处理器的新实例，确保处理器之间的隔离
        /// </summary>
        /// <param name="event">要发布的集成事件</param>
        /// <returns>表示异步操作的任务</returns>
        public async Task PublishAsync(IIntegrationEvent @event)
        {
            var eventName = @event.GetType().Name;
            _logger.LogInformation("Publishing integration event: {EventName}", eventName);

            // 检查是否有订阅者处理此事件
            if (!_subsManager.HasSubscriptionsForEvent(eventName))
            {
                _logger.LogWarning("No subscription for integration event: {EventName}", eventName);
                return;
            }

            // 获取所有订阅该事件的处理器
            var handlers = _subsManager.GetHandlersForEvent(eventName);
            
            // 为每个处理器创建一个独立的作用域，并调用其HandleAsync方法
            foreach (var subscription in handlers)
            {
                // 为每个处理器创建新的作用域，确保处理器间的隔离性
                using var scope = _serviceProvider.CreateScope();
                var handler = scope.ServiceProvider.GetService(subscription.HandlerType);
                
                if (handler == null) continue;
                
                // 使用反射调用处理器的HandleAsync方法
                var eventType = _subsManager.GetEventTypeByName(eventName);
                var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                var handleMethod = concreteType.GetMethod("HandleAsync");
                
                try
                {
                    await (Task)handleMethod.Invoke(handler, new object[] { @event });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error handling integration event {EventName}", eventName);
                }
            }
        }

        /// <summary>
        /// 订阅集成事件
        /// 注册特定类型事件的处理器
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <typeparam name="TH">事件处理器类型</typeparam>
        public void Subscribe<T, TH>()
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = typeof(T).Name;
            
            _logger.LogInformation("Subscribing to event {EventName} with {EventHandler}", eventName, typeof(TH).Name);
            
            // 在订阅管理器中添加事件订阅
            _subsManager.AddSubscription<T, TH>();
        }

        /// <summary>
        /// 取消订阅集成事件
        /// 移除特定类型事件的处理器
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <typeparam name="TH">事件处理器类型</typeparam>
        public void Unsubscribe<T, TH>()
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = typeof(T).Name;
            
            _logger.LogInformation("Unsubscribing from event {EventName}", eventName);
            
            // 从订阅管理器中移除事件订阅
            _subsManager.RemoveSubscription<T, TH>();
        }
    }
} 