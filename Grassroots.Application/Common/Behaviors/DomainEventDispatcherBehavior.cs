using Grassroots.Application.Common.Interfaces;
using Grassroots.Domain.Entities;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Grassroots.Application.Common.Behaviors
{
    /// <summary>
    /// 领域事件分发器行为
    /// </summary>
    public class DomainEventDispatcherBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IDomainEventService _domainEventService;
        private readonly IApplicationDbContext _dbContext;

        public DomainEventDispatcherBehavior(
            IDomainEventService domainEventService,
            IApplicationDbContext dbContext)
        {
            _domainEventService = domainEventService;
            _dbContext = dbContext;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // 执行处理程序
            var response = await next();
            
            // 获取DbContext跟踪的所有聚合根中的领域事件
            var domainEntities = _dbContext.GetDomainEntities()
                .OfType<AggregateRoot>()
                .Where(x => x.DomainEvents.Any())
                .ToList();
            
            // 获取所有领域事件
            var domainEvents = domainEntities
                .SelectMany(x => x.DomainEvents)
                .ToList();
            
            // 清除所有聚合根中的领域事件
            domainEntities.ForEach(entity => entity.ClearDomainEvents());
            
            // 分发所有领域事件
            foreach (var domainEvent in domainEvents)
            {
                await _domainEventService.PublishAsync(domainEvent);
            }
            
            return response;
        }
    }
} 