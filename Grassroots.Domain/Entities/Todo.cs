using System;
using Grassroots.Domain.AggregateRoots;

namespace Grassroots.Domain.Entities
{
    /// <summary>
    /// 待办事项实体
    /// </summary>
    public class Todo : AggregateRoot
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// 是否完成
        /// </summary>
        public bool IsCompleted { get; private set; }

        /// <summary>
        /// 截止日期
        /// </summary>
        public DateTime? DueDate { get; private set; }

        /// <summary>
        /// 默认构造函数（供EF Core使用）
        /// </summary>
        protected Todo()
        {
        }

        /// <summary>
        /// 创建待办事项
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="description">描述</param>
        /// <param name="dueDate">截止日期</param>
        public Todo(string title, string description, DateTime? dueDate = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("标题不能为空", nameof(title));

            Title = title;
            Description = description;
            DueDate = dueDate;
            IsCompleted = false;
        }

        /// <summary>
        /// 完成待办事项
        /// </summary>
        public void MarkAsCompleted()
        {
            IsCompleted = true;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// 更新待办事项
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="description">描述</param>
        /// <param name="dueDate">截止日期</param>
        public void Update(string title, string description, DateTime? dueDate)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("标题不能为空", nameof(title));

            Title = title;
            Description = description;
            DueDate = dueDate;
            UpdatedAt = DateTime.UtcNow;
        }
    }
} 