namespace Grassroots.Domain.Interfaces;

/// <summary>
/// 工作单元接口，用于协调多个仓储的事务一致性
/// 实现工作单元模式，将多个仓储操作封装在一个事务中
/// 确保所有操作要么全部成功，要么全部回滚，维护数据一致性
/// 提供对多个仓储的统一访问点和事务管理机制
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// 获取指定类型的仓储
    /// 工厂方法，用于获取与特定实体类型对应的仓储实例
    /// 允许在一个工作单元中处理多种类型的实体
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <returns>对应实体的仓储接口</returns>
    IRepository<TEntity> Repository<TEntity>() where TEntity : class;
    
    /// <summary>
    /// 保存所有更改
    /// 将所有仓储中的更改作为一个事务提交到数据库
    /// 在提交前可以触发领域事件处理等操作
    /// </summary>
    /// <returns>影响的行数</returns>
    Task<int> SaveChangesAsync();
    
    /// <summary>
    /// 开始一个新的事务
    /// 用于需要显式控制事务范围的场景
    /// 如长时间运行的业务流程或分布式事务
    /// </summary>
    /// <returns>事务对象，可用于后续的提交或回滚操作</returns>
    Task<IDbContextTransaction> BeginTransactionAsync();
    
    /// <summary>
    /// 提交事务
    /// 将事务中的所有更改永久保存到数据库
    /// 只有在调用此方法后，更改才会对其他会话可见
    /// </summary>
    /// <param name="transaction">要提交的事务对象，必须是由BeginTransactionAsync创建的</param>
    Task CommitTransactionAsync(IDbContextTransaction transaction);
    
    /// <summary>
    /// 回滚事务
    /// 撤销事务中的所有更改，恢复到事务开始前的状态
    /// 通常在检测到业务规则违反或发生异常时调用
    /// </summary>
    /// <param name="transaction">要回滚的事务对象，必须是由BeginTransactionAsync创建的</param>
    Task RollbackTransactionAsync(IDbContextTransaction transaction);
}

/// <summary>
/// 数据库上下文事务接口
/// 封装底层数据库事务，抽象不同数据库的事务实现
/// 提供一致的事务操作API，无需关心具体的数据库类型
/// </summary>
public interface IDbContextTransaction : IDisposable
{
    /// <summary>
    /// 提交事务
    /// 确认事务中的所有更改，并将其写入数据库
    /// </summary>
    Task CommitAsync();
    
    /// <summary>
    /// 回滚事务
    /// 撤销事务中的所有更改，不会修改数据库
    /// </summary>
    Task RollbackAsync();
} 