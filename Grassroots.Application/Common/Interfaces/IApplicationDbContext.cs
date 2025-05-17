using Grassroots.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Grassroots.Application.Common.Interfaces
{
    /// <summary>
    /// 应用数据库上下文接口
    /// </summary>
    public interface IApplicationDbContext
    {
        /// <summary>
        /// 保存更改
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>影响的行数</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        
        /// <summary>
        /// 获取数据库上下文跟踪的所有领域实体
        /// </summary>
        /// <returns>领域实体集合</returns>
        IEnumerable<BaseEntity> GetDomainEntities();
    }
} 