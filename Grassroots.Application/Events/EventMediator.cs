using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grassroots.Domain.AggregateRoots;
using Grassroots.Domain.Events;

namespace Grassroots.Application.Events
{
    /// <summary>
    /// 事件中介者，负责协调领域事件的处理和发布
    /// </summary>
    public class EventMediator : IEventMediator
    {
        private readonly IDomainEventBus _domainEventBus;
        private readonly IEventStore _eventStore;

        public EventMediator(IDomainEventBus domainEventBus, IEventStore eventStore)
        {
            _domainEventBus = domainEventBus;
            _eventStore = eventStore;
        }

        /// <summary>
        /// 发布聚合根中的所有领域事件
        /// </summary>
        /// <param name="aggregateRoot">聚合根</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>任务</returns>
        public async Task PublishEventsAsync(AggregateRoot aggregateRoot, CancellationToken cancellationToken = default)
        {
            var domainEvents = aggregateRoot.DomainEvents.ToList();
            if (domainEvents.Any())
            {
                // 保存事件到事件存储
                await _eventStore.SaveEventsAsync(
                    aggregateRoot.Id.ToString(),
                    domainEvents,
                    aggregateRoot.Version,
                    cancellationToken);

                // 发布领域事件
                foreach (var domainEvent in domainEvents)
                {
                    await _domainEventBus.PublishAsync(domainEvent, cancellationToken);
                }

                // 增加聚合根版本
                aggregateRoot.IncrementVersion(domainEvents.Count);

                // 清除已处理的事件
                aggregateRoot.ClearDomainEvents();
            }
        }

        /// <summary>
        /// 从事件存储中加载聚合根
        /// </summary>
        /// <typeparam name="T">聚合根类型</typeparam>
        /// <param name="aggregateId">聚合根ID</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>聚合根</returns>
        public async Task<T> LoadAggregateAsync<T>(string aggregateId, CancellationToken cancellationToken = default) where T : AggregateRoot, new()
        {
            // 从事件存储中获取事件
            var events = await _eventStore.GetEventsAsync(aggregateId, cancellationToken);
            if (!events.Any())
            {
                return null;
            }

            // 创建聚合根并应用事件
            var aggregate = new T();
            aggregate.ApplyEvents(events);
            return aggregate;
        }
    }
} 