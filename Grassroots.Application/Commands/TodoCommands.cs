using System;

namespace Grassroots.Application.Commands
{
    /// <summary>
    /// 创建待办事项命令
    /// </summary>
    public class CreateTodoCommand : ICommand<Guid>
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
        /// 截止日期
        /// </summary>
        public DateTime? DueDate { get; set; }
    }

    /// <summary>
    /// 更新待办事项命令
    /// </summary>
    public class UpdateTodoCommand : ICommand
    {
        /// <summary>
        /// 唯一标识符
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 截止日期
        /// </summary>
        public DateTime? DueDate { get; set; }
    }

    /// <summary>
    /// 标记待办事项为已完成命令
    /// </summary>
    public class CompleteTodoCommand : ICommand
    {
        /// <summary>
        /// 唯一标识符
        /// </summary>
        public Guid Id { get; set; }
    }

    /// <summary>
    /// 删除待办事项命令
    /// </summary>
    public class DeleteTodoCommand : ICommand
    {
        /// <summary>
        /// 唯一标识符
        /// </summary>
        public Guid Id { get; set; }
    }
} 