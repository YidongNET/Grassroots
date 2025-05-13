using System;
using System.Threading;
using System.Threading.Tasks;
using Grassroots.Model.Entities;
using Grassroots.Domain.Repositories;

namespace Grassroots.Application.Commands.Handlers
{
    /// <summary>
    /// 创建待办事项命令处理器
    /// </summary>
    public class CreateTodoCommandHandler : ICommandHandler<CreateTodoCommand, Guid>
    {
        private readonly IRepository<Todo> _repository;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">仓储</param>
        public CreateTodoCommandHandler(IRepository<Todo> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// 处理命令
        /// </summary>
        /// <param name="command">命令</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>创建的待办事项ID</returns>
        public async Task<Guid> HandleAsync(CreateTodoCommand command, CancellationToken cancellationToken = default)
        {
            var todo = new Todo
            {
                Title = command.Title,
                Description = command.Description,
                DueDate = command.DueDate
            };
            await _repository.AddAsync(todo, cancellationToken);
            return todo.Id;
        }
    }

    /// <summary>
    /// 更新待办事项命令处理器
    /// </summary>
    public class UpdateTodoCommandHandler : ICommandHandler<UpdateTodoCommand>
    {
        private readonly IRepository<Todo> _repository;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">仓储</param>
        public UpdateTodoCommandHandler(IRepository<Todo> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// 处理命令
        /// </summary>
        /// <param name="command">命令</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>操作结果</returns>
        public async Task HandleAsync(UpdateTodoCommand command, CancellationToken cancellationToken = default)
        {
            var todo = await _repository.GetByIdAsync(command.Id, cancellationToken);
            if (todo == null)
                throw new ArgumentException($"待办事项不存在: {command.Id}", nameof(command.Id));

            todo.Title = command.Title;
            todo.Description = command.Description;
            todo.DueDate = command.DueDate;
            
            await _repository.UpdateAsync(todo, cancellationToken);
        }
    }

    /// <summary>
    /// 标记待办事项为已完成命令处理器
    /// </summary>
    public class CompleteTodoCommandHandler : ICommandHandler<CompleteTodoCommand>
    {
        private readonly IRepository<Todo> _repository;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">仓储</param>
        public CompleteTodoCommandHandler(IRepository<Todo> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// 处理命令
        /// </summary>
        /// <param name="command">命令</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>操作结果</returns>
        public async Task HandleAsync(CompleteTodoCommand command, CancellationToken cancellationToken = default)
        {
            var todo = await _repository.GetByIdAsync(command.Id, cancellationToken);
            if (todo == null)
                throw new ArgumentException($"待办事项不存在: {command.Id}", nameof(command.Id));

            todo.IsCompleted = true;
            await _repository.UpdateAsync(todo, cancellationToken);
        }
    }

    /// <summary>
    /// 删除待办事项命令处理器
    /// </summary>
    public class DeleteTodoCommandHandler : ICommandHandler<DeleteTodoCommand>
    {
        private readonly IRepository<Todo> _repository;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">仓储</param>
        public DeleteTodoCommandHandler(IRepository<Todo> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// 处理命令
        /// </summary>
        /// <param name="command">命令</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>操作结果</returns>
        public async Task HandleAsync(DeleteTodoCommand command, CancellationToken cancellationToken = default)
        {
            var todo = await _repository.GetByIdAsync(command.Id, cancellationToken);
            if (todo == null)
                throw new ArgumentException($"待办事项不存在: {command.Id}", nameof(command.Id));

            await _repository.DeleteAsync(todo, cancellationToken);
        }
    }
} 