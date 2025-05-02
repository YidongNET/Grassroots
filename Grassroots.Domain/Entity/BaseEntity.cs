using System;

namespace Grassroots.Domain.Entity
{
    /// <summary>
    /// 所有实体的基类
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// 唯一标识符
        /// </summary>
        public Guid Id { get; protected set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        protected BaseEntity()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }
    }
} 