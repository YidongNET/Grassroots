using MediatR;

namespace Grassroots.Domain.Events;

/// <summary>
/// 领域事件接口
/// 领域事件代表系统中发生的重要事情，通常是过去时态的动词
/// 事件用于解耦系统组件，允许不同聚合/服务之间进行通信，而不产生直接依赖
/// 实现了MediatR的INotification接口，用于通过中介者模式发布事件
/// </summary>
public interface IDomainEvent : INotification
{
    /// <summary>
    /// 事件ID
    /// 全局唯一标识符，用于跟踪和关联事件
    /// 即使事件内容相同，每个事件实例也应有唯一ID
    /// </summary>
    Guid EventId { get; }
    
    /// <summary>
    /// 事件发生时间
    /// 记录事件实际发生的时间点，用于时间序列分析和事件排序
    /// 通常采用UTC时间格式，避免时区问题
    /// </summary>
    DateTime OccurredOn { get; }
    
    /// <summary>
    /// 事件类型名称
    /// 通常为事件类的全名，用于事件序列化和反序列化
    /// 在事件溯源系统中用于正确映射事件类型
    /// </summary>
    string EventType { get; }
} 