using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        /// 获取所有实体
        /// </summary>
        /// <returns>实体集合</returns>
        IQueryable<T> GetAll();
        
        /// <summary>
        /// 根据条件获取实体
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns>实体集合</returns>
        IQueryable<T> Find(Expression<Func<T, bool>> predicate);
        
        /// <summary>
        /// 根据ID获取实体
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <returns>实体</returns>
        Task<T> GetByIdAsync(Guid id);
        
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>实体</returns>
        Task<T> AddAsync(T entity);
        
        /// <summary>
        /// 添加多个实体
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <returns>任务</returns>
        Task AddRangeAsync(IEnumerable<T> entities);
        
        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>任务</returns>
        Task UpdateAsync(T entity);
        
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>任务</returns>
        Task DeleteAsync(T entity);
        
        /// <summary>
        /// 根据ID删除实体
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <returns>任务</returns>
        Task DeleteAsync(Guid id);
        
        /// <summary>
        /// 删除多个实体
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <returns>任务</returns>
        Task DeleteRangeAsync(IEnumerable<T> entities);
        
        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns>受影响的行数</returns>
        Task<int> SaveChangesAsync();
    }
} 