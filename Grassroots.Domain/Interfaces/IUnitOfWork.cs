namespace Grassroots.Domain.Interfaces;

/// <summary>
/// 工作单元接口，用于协调多个仓储的事务一致性
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// 获取指定类型的仓储
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <returns>对应实体的仓储接口</returns>
    IRepository<TEntity> Repository<TEntity>() where TEntity : class;
    
    /// <summary>
    /// 保存所有更改
    /// </summary>
    /// <returns>影响的行数</returns>
    Task<int> SaveChangesAsync();
    
    /// <summary>
    /// 开始一个新的事务
    /// </summary>
    /// <returns>事务对象</returns>
    Task<IDbContextTransaction> BeginTransactionAsync();
    
    /// <summary>
    /// 提交事务
    /// </summary>
    /// <param name="transaction">要提交的事务</param>
    Task CommitTransactionAsync(IDbContextTransaction transaction);
    
    /// <summary>
    /// 回滚事务
    /// </summary>
    /// <param name="transaction">要回滚的事务</param>
    Task RollbackTransactionAsync(IDbContextTransaction transaction);
}

/// <summary>
/// 数据库上下文事务接口
/// </summary>
public interface IDbContextTransaction : IDisposable
{
    /// <summary>
    /// 提交事务
    /// </summary>
    Task CommitAsync();
    
    /// <summary>
    /// 回滚事务
    /// </summary>
    Task RollbackAsync();
} 