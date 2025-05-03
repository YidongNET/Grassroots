using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grassroots.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Grassroots.Infrastructure.Data
{
    public class GrassrootsDbContext : DbContext
    {
        public GrassrootsDbContext(DbContextOptions<GrassrootsDbContext> options) : base(options)
        {
        }

        public DbSet<Todo> Todos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 配置全局查询过滤器，过滤软删除的实体
            modelBuilder.Entity<Todo>().HasQueryFilter(e => !e.IsDeleted);

            // 配置实体的表名(在这里，我们使用复数形式)
            modelBuilder.Entity<Todo>().ToTable("Todos");
        }

        public override int SaveChanges()
        {
            ApplyAuditInfo();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditInfo();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyAuditInfo()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (BaseEntity)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    entity.Id = entity.Id == Guid.Empty ? Guid.NewGuid() : entity.Id;
                    entity.CreatedAt = DateTime.UtcNow;
                    // 这里应该从认证系统获取当前用户ID
                    // entity.CreatedBy = currentUserId;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Property(nameof(BaseEntity.CreatedAt)).IsModified = false;
                    entry.Property(nameof(BaseEntity.CreatedBy)).IsModified = false;
                    entity.UpdatedAt = DateTime.UtcNow;
                    // 这里应该从认证系统获取当前用户ID
                    // entity.UpdatedBy = currentUserId;
                }
            }
        }
    }
} 