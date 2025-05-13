using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Grassroots.Model.Entities;

namespace Grassroots.Domain.Repositories
{
    /// <summary>
    /// 通用仓储接口
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    public interface IRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// 根据ID获取实体
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
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 根据条件查找实体
        /// </summary>
        /// <param name="predicate">条件表达式</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>符合条件的实体列表</returns>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>操作结果</returns>
        Task AddAsync(T entity, CancellationToken cancellationToken = default);

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

        /// <summary>
        /// 检查实体是否存在
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>是否存在</returns>
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取查询对象
        /// </summary>
        /// <returns>查询对象</returns>
        IQueryable<T> AsQueryable();
    }
} 