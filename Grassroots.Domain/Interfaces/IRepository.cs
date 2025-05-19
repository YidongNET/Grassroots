using System.Linq.Expressions;

namespace Grassroots.Domain.Interfaces;

/// <summary>
/// 通用仓储接口
/// 实现仓储模式，为领域实体提供统一的持久化和查询操作
/// 隐藏数据访问的具体实现细节，使领域层不直接依赖于数据访问技术
/// 支持CRUD操作和各种查询方式，包括条件查询、排序和包含关系
/// </summary>
/// <typeparam name="T">实体类型，通常是领域实体或聚合根</typeparam>
public interface IRepository<T> where T : class
{
    // 查询方法

    /// <summary>
    /// 根据ID获取实体
    /// </summary>
    /// <param name="id">实体ID</param>
    /// <returns>找到的实体，如果不存在则返回null</returns>
    Task<T?> GetByIdAsync(object id);

    /// <summary>
    /// 获取所有实体
    /// 注意：在大型数据集上应谨慎使用，可能导致性能问题
    /// </summary>
    /// <returns>所有实体的只读列表</returns>
    Task<IReadOnlyList<T>> GetAllAsync();

    /// <summary>
    /// 根据条件查询实体
    /// </summary>
    /// <param name="predicate">过滤条件表达式</param>
    /// <returns>符合条件的实体列表</returns>
    Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// 高级查询方法，支持条件、排序和包含关系
    /// </summary>
    /// <param name="predicate">过滤条件表达式</param>
    /// <param name="orderBy">排序表达式</param>
    /// <param name="includeString">包含关系的导航属性路径</param>
    /// <param name="disableTracking">是否禁用实体跟踪，提高只读查询性能</param>
    /// <returns>符合条件的实体列表</returns>
    Task<IReadOnlyList<T>> GetAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        string? includeString = null,
        bool disableTracking = true);

    /// <summary>
    /// 高级查询方法，支持条件、排序和多个包含关系
    /// </summary>
    /// <param name="predicate">过滤条件表达式</param>
    /// <param name="orderBy">排序表达式</param>
    /// <param name="includes">包含关系的导航属性表达式列表</param>
    /// <param name="disableTracking">是否禁用实体跟踪</param>
    /// <returns>符合条件的实体列表</returns>
    Task<IReadOnlyList<T>> GetAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        List<Expression<Func<T, object>>>? includes = null,
        bool disableTracking = true);

    /// <summary>
    /// 获取符合条件的第一个实体
    /// </summary>
    /// <param name="predicate">过滤条件表达式</param>
    /// <returns>符合条件的第一个实体，如果不存在则返回null</returns>
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// 计算符合条件的实体数量
    /// </summary>
    /// <param name="predicate">过滤条件表达式，为null时计算所有实体</param>
    /// <returns>符合条件的实体数量</returns>
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);

    // 写入方法

    /// <summary>
    /// 添加新实体
    /// </summary>
    /// <param name="entity">要添加的实体实例</param>
    /// <returns>添加后的实体（可能包含生成的ID等信息）</returns>
    Task<T> AddAsync(T entity);

    /// <summary>
    /// 批量添加多个实体
    /// </summary>
    /// <param name="entities">要添加的实体集合</param>
    /// <returns>添加后的实体集合</returns>
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);

    /// <summary>
    /// 更新现有实体
    /// </summary>
    /// <param name="entity">要更新的实体，必须包含有效的ID</param>
    Task UpdateAsync(T entity);

    /// <summary>
    /// 批量更新多个实体
    /// </summary>
    /// <param name="entities">要更新的实体集合</param>
    Task UpdateRangeAsync(IEnumerable<T> entities);

    /// <summary>
    /// 删除实体
    /// </summary>
    /// <param name="entity">要删除的实体</param>
    Task DeleteAsync(T entity);

    /// <summary>
    /// 批量删除多个实体
    /// </summary>
    /// <param name="entities">要删除的实体集合</param>
    Task DeleteRangeAsync(IEnumerable<T> entities);
    
    /// <summary>
    /// 保存对数据库的所有更改
    /// 将所有挂起的更改提交到数据库
    /// </summary>
    /// <returns>受影响的行数</returns>
    Task<int> SaveChangesAsync();
} 