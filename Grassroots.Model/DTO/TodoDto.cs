using System;

namespace Grassroots.Model.DTO
{
    /// <summary>
    /// 待办事项数据传输对象
    /// </summary>
    public class TodoDto : BaseDto
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 是否完成
        /// </summary>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// 截止日期
        /// </summary>
        public DateTime? DueDate { get; set; }
    }
} 