using Grassroots.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Grassroots.Infrastructure.Data.Configurations
{
    /// <summary>
    /// 待办事项实体配置
    /// </summary>
    public class TodoConfiguration : IEntityTypeConfiguration<Todo>
    {
        /// <summary>
        /// 配置实体
        /// </summary>
        /// <param name="builder">实体类型构建器</param>
        public void Configure(EntityTypeBuilder<Todo> builder)
        {
            // 基本属性配置
            builder.Property(t => t.Title).IsRequired().HasMaxLength(200);
            builder.Property(t => t.Description).HasMaxLength(1000);
            builder.Property(t => t.IsCompleted).IsRequired();
            builder.Property(t => t.DueDate).IsRequired(false);
            
            // 设置表名
            builder.ToTable("Todos");
        }
    }
} 