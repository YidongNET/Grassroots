using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grassroots.Model.Entities;
using Grassroots.Domain.Repositories;
using Grassroots.Model.DTO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Grassroots.Application.Queries.Handlers
{
    /// <summary>
    /// 获取所有待办事项查询处理器
    /// </summary>
    public class GetAllTodosQueryHandler : IQueryHandler<GetAllTodosQuery, IEnumerable<TodoDto>>
    {
        private readonly IRepository<Todo> _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">仓储</param>
        /// <param name="mapper">映射器</param>
        public GetAllTodosQueryHandler(IRepository<Todo> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// 处理查询
        /// </summary>
        /// <param name="query">查询</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>查询结果</returns>
        public async Task<IEnumerable<TodoDto>> HandleAsync(GetAllTodosQuery query, CancellationToken cancellationToken = default)
        {
            var todos = await _repository.AsQueryable().ToListAsync(cancellationToken);
            return _mapper.Map<List<TodoDto>>(todos);
        }
    }

    /// <summary>
    /// 根据ID获取待办事项查询处理器
    /// </summary>
    public class GetTodoByIdQueryHandler : IQueryHandler<GetTodoByIdQuery, TodoDto>
    {
        private readonly IRepository<Todo> _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">仓储</param>
        /// <param name="mapper">映射器</param>
        public GetTodoByIdQueryHandler(IRepository<Todo> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// 处理查询
        /// </summary>
        /// <param name="query">查询</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>查询结果</returns>
        public async Task<TodoDto> HandleAsync(GetTodoByIdQuery query, CancellationToken cancellationToken = default)
        {
            var todo = await _repository.GetByIdAsync(query.Id, cancellationToken);
            if (todo == null)
                throw new ArgumentException($"待办事项不存在: {query.Id}", nameof(query.Id));

            return _mapper.Map<TodoDto>(todo);
        }
    }

    /// <summary>
    /// 获取已完成待办事项查询处理器
    /// </summary>
    public class GetCompletedTodosQueryHandler : IQueryHandler<GetCompletedTodosQuery, IEnumerable<TodoDto>>
    {
        private readonly IRepository<Todo> _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">仓储</param>
        /// <param name="mapper">映射器</param>
        public GetCompletedTodosQueryHandler(IRepository<Todo> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// 处理查询
        /// </summary>
        /// <param name="query">查询</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>查询结果</returns>
        public async Task<IEnumerable<TodoDto>> HandleAsync(GetCompletedTodosQuery query, CancellationToken cancellationToken = default)
        {
            var todos = await _repository.AsQueryable().Where(t => t.IsCompleted).ToListAsync(cancellationToken);
            return _mapper.Map<List<TodoDto>>(todos);
        }
    }

    /// <summary>
    /// 获取未完成待办事项查询处理器
    /// </summary>
    public class GetIncompleteTodosQueryHandler : IQueryHandler<GetIncompleteTodosQuery, IEnumerable<TodoDto>>
    {
        private readonly IRepository<Todo> _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">仓储</param>
        /// <param name="mapper">映射器</param>
        public GetIncompleteTodosQueryHandler(IRepository<Todo> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// 处理查询
        /// </summary>
        /// <param name="query">查询</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>查询结果</returns>
        public async Task<IEnumerable<TodoDto>> HandleAsync(GetIncompleteTodosQuery query, CancellationToken cancellationToken = default)
        {
            var todos = await _repository.AsQueryable().Where(t => !t.IsCompleted).ToListAsync(cancellationToken);
            return _mapper.Map<List<TodoDto>>(todos);
        }
    }
} 