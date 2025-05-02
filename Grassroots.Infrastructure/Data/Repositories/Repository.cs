using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Grassroots.Domain.Entity;
using Grassroots.Domain.Repositories;
using Grassroots.Infrastructure.Data.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Grassroots.Infrastructure.Data.Repositories
{
    /// <summary>
    /// 通用仓储实现
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly ApplicationDbContext _dbContext;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        public Repository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>实体</returns>
        public async Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<T>().FindAsync(new object[] { id }, cancellationToken);
        }

        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>实体列表</returns>
        public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<T>().ToListAsync(cancellationToken);
        }

        /// <summary>
        /// 根据条件查询实体
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>实体列表</returns>
        public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<T>().Where(predicate).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>操作结果</returns>
        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbContext.Set<T>().AddAsync(entity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return entity;
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>操作结果</returns>
        public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>操作结果</returns>
        public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
} 