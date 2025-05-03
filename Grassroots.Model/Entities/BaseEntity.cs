using System;

namespace Grassroots.Model.Entities
{
    /// <summary>
    /// 基础实体类，所有实体都应该继承此类
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// 唯一标识符
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// 创建者ID
        /// </summary>
        public Guid? CreatedBy { get; set; }

        /// <summary>
        /// 最后更新者ID
        /// </summary>
        public Guid? UpdatedBy { get; set; }

        /// <summary>
        /// 是否已删除（软删除）
        /// </summary>
        public bool IsDeleted { get; set; } = false;
    }
} 