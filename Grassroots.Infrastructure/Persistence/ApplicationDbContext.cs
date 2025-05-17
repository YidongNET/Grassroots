using Grassroots.Application.Common.Interfaces;
using Grassroots.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Grassroots.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(builder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        
        /// <summary>
        /// 获取数据库上下文跟踪的所有领域实体
        /// </summary>
        /// <returns>领域实体集合</returns>
        public IEnumerable<BaseEntity> GetDomainEntities()
        {
            return ChangeTracker.Entries<BaseEntity>()
                .Where(x => x.Entity != null)
                .Select(x => x.Entity);
        }
    }
} 