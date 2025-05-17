using Microsoft.EntityFrameworkCore;

namespace Grassroots.Infrastructure.EventSourcing
{
    /// <summary>
    /// 事件存储数据库上下文
    /// </summary>
    public class EventStoreDbContext : DbContext
    {
        public EventStoreDbContext(DbContextOptions<EventStoreDbContext> options) : base(options)
        {
        }
        
        /// <summary>
        /// 事件描述符集合
        /// </summary>
        public DbSet<EventDescriptor> EventDescriptors { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EventDescriptor>(builder =>
            {
                builder.HasKey(e => e.Id);
                builder.Property(e => e.AggregateId).IsRequired();
                builder.Property(e => e.EventType).IsRequired();
                builder.Property(e => e.EventData).IsRequired();
                builder.Property(e => e.Version).IsRequired();
                builder.Property(e => e.Timestamp).IsRequired();
                
                builder.HasIndex(e => new { e.AggregateId, e.Version }).IsUnique();
            });
            
            base.OnModelCreating(modelBuilder);
        }
    }
} 