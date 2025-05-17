using Grassroots.Application.Common.Interfaces;
using Grassroots.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Grassroots.Infrastructure.Services
{
    /// <summary>
    /// 领域事件服务实现
    /// </summary>
    public class DomainEventService : IDomainEventService
    {
        private readonly ILogger<DomainEventService> _logger;
        private readonly IPublisher _mediator;
        private readonly IEventBus _eventBus;

        public DomainEventService(
            ILogger<DomainEventService> logger,
            IPublisher mediator,
            IEventBus eventBus)
        {
            _logger = logger;
            _mediator = mediator;
            _eventBus = eventBus;
        }

        /// <summary>
        /// 发布领域事件
        /// </summary>
        /// <param name="domainEvent">领域事件</param>
        public async Task PublishAsync(IDomainEvent domainEvent)
        {
            _logger.LogInformation("Publishing domain event. Event - {event}", domainEvent.GetType().Name);
            
            try
            {
                // 使用MediatR发布领域事件到内部处理程序
                await _mediator.Publish(domainEvent);
                
                // 如果事件实现了IIntegrationEvent接口，则发布为集成事件
                if (domainEvent is IIntegrationEvent integrationEvent)
                {
                    await _eventBus.PublishAsync(integrationEvent);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing domain event: {Event}", domainEvent.GetType().Name);
                throw;
            }
        }
    }
} 