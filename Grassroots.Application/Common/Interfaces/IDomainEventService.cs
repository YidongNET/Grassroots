using System.Threading.Tasks;
using Grassroots.Domain.Events;

namespace Grassroots.Application.Common.Interfaces
{
    /// <summary>
    /// 领域事件服务接口
    /// </summary>
    public interface IDomainEventService
    {
        /// <summary>
        /// 发布领域事件
        /// </summary>
        /// <param name="domainEvent">领域事件</param>
        Task PublishAsync(IDomainEvent domainEvent);
    }
} 