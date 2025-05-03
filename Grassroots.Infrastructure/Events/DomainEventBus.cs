using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grassroots.Domain.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Grassroots.Infrastructure.Events
{
    /// <summary>
    /// 领域事件总线实现
    /// </summary>
    public class DomainEventBus : IDomainEventBus
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<Type, List<Type>> _handlers = new Dictionary<Type, List<Type>>();

        public DomainEventBus(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 发布事件
        /// </summary>
        /// <param name="event">事件</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>任务</returns>
        public async Task PublishAsync(DomainEvent @event, CancellationToken cancellationToken = default)
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            var eventType = @event.GetType();

            if (_handlers.ContainsKey(eventType))
            {
                using var scope = _serviceProvider.CreateScope();
                
                var tasks = _handlers[eventType]
                    .Select(handlerType =>
                    {
                        var handler = scope.ServiceProvider.GetService(handlerType);
                        var methodInfo = handlerType.GetMethod("HandleAsync");
                        
                        if (handler == null || methodInfo == null)
                        {
                            return Task.CompletedTask;
                        }
                        
                        return (Task)methodInfo.Invoke(handler, new object[] { @event, cancellationToken });
                    })
                    .ToArray();

                await Task.WhenAll(tasks);
            }
        }

        /// <summary>
        /// 注册事件处理器
        /// </summary>
        /// <typeparam name="TEvent">事件类型</typeparam>
        /// <typeparam name="THandler">处理器类型</typeparam>
        public void Subscribe<TEvent, THandler>()
            where TEvent : DomainEvent
            where THandler : IDomainEventHandler<TEvent>
        {
            var eventType = typeof(TEvent);
            var handlerType = typeof(THandler);

            if (!_handlers.ContainsKey(eventType))
            {
                _handlers[eventType] = new List<Type>();
            }

            if (_handlers[eventType].Contains(handlerType))
            {
                throw new ArgumentException($"Handler Type {handlerType.Name} already registered for '{eventType.Name}'", nameof(handlerType));
            }

            _handlers[eventType].Add(handlerType);
        }
    }
} 