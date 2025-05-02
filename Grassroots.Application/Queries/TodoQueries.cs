using System;
using System.Collections.Generic;
using Grassroots.Model.DTO;

namespace Grassroots.Application.Queries
{
    /// <summary>
    /// 获取所有待办事项查询
    /// </summary>
    public class GetAllTodosQuery : IQuery<IEnumerable<TodoDto>>
    {
    }

    /// <summary>
    /// 根据ID获取待办事项查询
    /// </summary>
    public class GetTodoByIdQuery : IQuery<TodoDto>
    {
        /// <summary>
        /// 唯一标识符
        /// </summary>
        public Guid Id { get; set; }
    }

    /// <summary>
    /// 获取已完成待办事项查询
    /// </summary>
    public class GetCompletedTodosQuery : IQuery<IEnumerable<TodoDto>>
    {
    }

    /// <summary>
    /// 获取未完成待办事项查询
    /// </summary>
    public class GetIncompleteTodosQuery : IQuery<IEnumerable<TodoDto>>
    {
    }
} 