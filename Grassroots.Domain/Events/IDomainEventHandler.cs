using System;
using System.Threading;
using System.Threading.Tasks;
using Grassroots.Domain.Common;

namespace Grassroots.Domain.Events;

/// <summary>
/// 领域事件处理器接口
/// 定义了处理特定领域事件的契约
/// </summary>
/// <typeparam name="TDomainEvent">领域事件类型</typeparam>
public interface IDomainEventHandler<in TDomainEvent>
    where TDomainEvent : IDomainEvent
{
    /// <summary>
    /// 处理领域事件
    /// </summary>
    /// <param name="domainEvent">要处理的领域事件</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task HandleAsync(TDomainEvent domainEvent, CancellationToken cancellationToken = default);
} 