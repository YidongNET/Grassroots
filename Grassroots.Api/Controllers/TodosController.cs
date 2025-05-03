using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Grassroots.Application.Commands;
using Grassroots.Application.Queries;
using Grassroots.Model.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Grassroots.Domain.Repositories;
using Grassroots.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Grassroots.Api.Controllers
{
    /// <summary>
    /// 待办事项控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TodosController : ApiBaseController
    {
        private readonly IRepository<Todo> _todoRepository;

        public TodosController(IRepository<Todo> todoRepository)
        {
            _todoRepository = todoRepository;
        }

        /// <summary>
        /// 获取所有待办事项
        /// </summary>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>待办事项列表</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TodoDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TodoDto>>> GetAll(CancellationToken cancellationToken)
        {
            var query = new GetAllTodosQuery();
            var result = await QueryDispatcher.QueryAsync<GetAllTodosQuery, IEnumerable<TodoDto>>(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// 根据ID获取待办事项
        /// </summary>
        /// <param name="id">待办事项ID</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>待办事项</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TodoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TodoDto>> GetById(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetTodoByIdQuery { Id = id };
                var result = await QueryDispatcher.QueryAsync<GetTodoByIdQuery, TodoDto>(query, cancellationToken);
                return Ok(result);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// 获取已完成待办事项
        /// </summary>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>已完成待办事项列表</returns>
        [HttpGet("completed")]
        [ProducesResponseType(typeof(IEnumerable<TodoDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TodoDto>>> GetCompleted(CancellationToken cancellationToken)
        {
            var query = new GetCompletedTodosQuery();
            var result = await QueryDispatcher.QueryAsync<GetCompletedTodosQuery, IEnumerable<TodoDto>>(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// 获取未完成待办事项
        /// </summary>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>未完成待办事项列表</returns>
        [HttpGet("incomplete")]
        [ProducesResponseType(typeof(IEnumerable<TodoDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TodoDto>>> GetIncomplete(CancellationToken cancellationToken)
        {
            var query = new GetIncompleteTodosQuery();
            var result = await QueryDispatcher.QueryAsync<GetIncompleteTodosQuery, IEnumerable<TodoDto>>(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// 创建待办事项
        /// </summary>
        /// <param name="command">创建待办事项命令</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>创建的待办事项ID</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> Create(CreateTodoCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var result = await CommandDispatcher.SendAsync<CreateTodoCommand, Guid>(command, cancellationToken);
                return CreatedAtAction(nameof(GetById), new { id = result }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 更新待办事项
        /// </summary>
        /// <param name="id">待办事项ID</param>
        /// <param name="command">更新待办事项命令</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>操作结果</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, UpdateTodoCommand command, CancellationToken cancellationToken)
        {
            if (id != command.Id)
                return BadRequest("ID不匹配");

            try
            {
                await CommandDispatcher.SendAsync(command, cancellationToken);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                if (ex.ParamName == nameof(command.Id))
                    return NotFound();

                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 标记待办事项为已完成
        /// </summary>
        /// <param name="id">待办事项ID</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>操作结果</returns>
        [HttpPut("{id}/complete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Complete(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var command = new CompleteTodoCommand { Id = id };
                await CommandDispatcher.SendAsync(command, cancellationToken);
                return NoContent();
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// 删除待办事项
        /// </summary>
        /// <param name="id">待办事项ID</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>操作结果</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var command = new DeleteTodoCommand { Id = id };
                await CommandDispatcher.SendAsync(command, cancellationToken);
                return NoContent();
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }
    }
} 