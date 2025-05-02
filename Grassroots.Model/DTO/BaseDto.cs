using System;

namespace Grassroots.Model.DTO
{
    /// <summary>
    /// 基础数据传输对象
    /// </summary>
    public abstract class BaseDto
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
        /// 更新时间
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
} 