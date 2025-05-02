using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Grassroots.Domain.Entity;

namespace Grassroots.Domain.Repositories
{
    /// <summary>
    /// 通用仓储接口
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    public interface IRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>实体</returns>
        Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>实体列表</returns>
        Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 根据条件查询实体
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>实体列表</returns>
        Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>操作结果</returns>
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>操作结果</returns>
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>操作结果</returns>
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    }
} 