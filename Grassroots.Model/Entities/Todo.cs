using System;
using System.ComponentModel.DataAnnotations;

namespace Grassroots.Model.Entities
{
    /// <summary>
    /// 待办事项实体
    /// </summary>
    public class Todo : BaseEntity
    {
        /// <summary>
        /// 标题
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [MaxLength(1000)]
        public string Description { get; set; }

        /// <summary>
        /// 是否完成
        /// </summary>
        public bool IsCompleted { get; set; } = false;

        /// <summary>
        /// 优先级 (1-低, 2-中, 3-高)
        /// </summary>
        public int Priority { get; set; } = 2;

        /// <summary>
        /// 截止日期
        /// </summary>
        public DateTime? DueDate { get; set; }
    }
} 