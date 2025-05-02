using System.Threading;
using System.Threading.Tasks;
using Grassroots.Application.Queries;

namespace Grassroots.Application.Dispatchers
{
    /// <summary>
    /// 查询调度器接口
    /// </summary>
    public interface IQueryDispatcher
    {
        /// <summary>
        /// 发送查询
        /// </summary>
        /// <typeparam name="TQuery">查询类型</typeparam>
        /// <typeparam name="TResult">查询结果类型</typeparam>
        /// <param name="query">查询</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>查询结果</returns>
        Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default) 
            where TQuery : IQuery<TResult>;
    }
} 