using System;
using System.Threading;
using System.Threading.Tasks;
using Grassroots.Application.Dispatchers;
using Grassroots.Application.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Grassroots.Infrastructure.Queries
{
    /// <summary>
    /// 查询分发器实现
    /// </summary>
    public class QueryDispatcher : IQueryDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="serviceProvider">服务提供者</param>
        public QueryDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 发送查询
        /// </summary>
        /// <typeparam name="TQuery">查询类型</typeparam>
        /// <typeparam name="TResult">查询结果类型</typeparam>
        /// <param name="query">查询</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>查询结果</returns>
        public async Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
            where TQuery : IQuery<TResult>
        {
            var handler = _serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResult>>();
            return await handler.HandleAsync(query, cancellationToken);
        }
    }
} 