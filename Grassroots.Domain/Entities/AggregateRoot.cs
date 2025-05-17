using System.Collections.Generic;
using Grassroots.Domain.Events;

namespace Grassroots.Domain.Entities
{
    /// <summary>
    /// 聚合根基类，支持领域事件
    /// 聚合根是领域驱动设计(DDD)中的核心概念，代表一个业务实体集合的根节点
    /// 所有对聚合内部对象的修改必须通过聚合根进行，以保证业务规则的一致性
    /// </summary>
    public abstract class AggregateRoot : BaseEntity
    {
        private readonly List<IDomainEvent> _domainEvents = new();
        
        /// <summary>
        /// 聚合版本，用于乐观并发控制和事件溯源
        /// 每次聚合状态变更时递增
        /// </summary>
        public int Version { get; protected set; } = 0;
        
        /// <summary>
        /// 获取已注册的领域事件
        /// 返回只读集合，防止外部代码直接修改事件列表
        /// </summary>
        public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
        
        /// <summary>
        /// 添加领域事件
        /// 在聚合状态发生变化时，应通过此方法记录相关领域事件
        /// 同时自动增加聚合版本号
        /// </summary>
        /// <param name="domainEvent">要添加的领域事件</param>
        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
            Version++;
        }
        
        /// <summary>
        /// 清除领域事件
        /// 通常在事件被处理后调用，避免重复处理
        /// </summary>
        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
} 