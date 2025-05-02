using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grassroots.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Grassroots.Infrastructure.Data.DbContext
{
    /// <summary>
    /// 应用程序数据库上下文
    /// </summary>
    public class ApplicationDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options">数据库上下文选项</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// 配置模型
        /// </summary>
        /// <param name="modelBuilder">模型构建器</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // 应用实体配置
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            
            // 配置所有继承自BaseEntity的实体
            foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                .Where(t => typeof(BaseEntity).IsAssignableFrom(t.ClrType)))
            {
                // 配置Id属性为主键
                modelBuilder.Entity(entityType.ClrType)
                    .HasKey(nameof(BaseEntity.Id));
                
                // 配置CreatedAt属性必填
                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(BaseEntity.CreatedAt))
                    .IsRequired();
                
                // 配置UpdatedAt属性可空
                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(BaseEntity.UpdatedAt))
                    .IsRequired(false);
            }
        }

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>操作结果</returns>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // 设置实体的创建时间和更新时间
            var entries = ChangeTracker.Entries<BaseEntity>();
            
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
} 