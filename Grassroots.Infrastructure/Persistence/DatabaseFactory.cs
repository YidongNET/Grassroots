using Grassroots.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Grassroots.Infrastructure.Persistence
{
    /// <summary>
    /// 数据库提供程序类型
    /// 定义系统支持的所有数据库类型
    /// </summary>
    public enum DatabaseProviderType
    {
        /// <summary>SQL Server 数据库</summary>
        SqlServer,
        
        /// <summary>PostgreSQL 数据库</summary>
        PostgreSQL,
        
        /// <summary>MySQL 数据库</summary>
        MySQL,
        
        /// <summary>SQLite 数据库</summary>
        SQLite,
        
        /// <summary>内存数据库，主要用于测试</summary>
        InMemory
    }

    /// <summary>
    /// 数据库工厂类，用于创建不同数据库提供程序的DbContext
    /// 实现了工厂模式，允许应用程序在运行时动态切换数据库提供程序
    /// 无需修改代码，仅通过配置文件即可更改底层数据库
    /// 支持多种数据库，包括：SQL Server, PostgreSQL, MySQL, SQLite和内存数据库
    /// </summary>
    public class DatabaseFactory
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configuration">应用程序配置，用于读取数据库设置</param>
        public DatabaseFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 配置数据库上下文的提供程序
        /// 根据配置选择合适的数据库提供程序并进行配置
        /// 这是实现多数据库支持的核心方法
        /// </summary>
        /// <param name="options">DbContext选项构建器</param>
        public void ConfigureDbContext(DbContextOptionsBuilder options)
        {
            var providerType = GetProviderTypeFromConfig();
            var connectionString = GetConnectionString();

            switch (providerType)
            {
                case DatabaseProviderType.SqlServer:
                    options.UseSqlServer(connectionString, 
                        b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
                    break;
                
                case DatabaseProviderType.PostgreSQL:
                    options.UseNpgsql(connectionString, 
                        b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
                    break;
                
                case DatabaseProviderType.MySQL:
                    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), 
                        b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
                    break;
                
                case DatabaseProviderType.SQLite:
                    options.UseSqlite(connectionString, 
                        b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
                    break;
                
                case DatabaseProviderType.InMemory:
                default:
                    options.UseInMemoryDatabase("GrassrootsDb")
                          .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));
                    break;
            }
        }

        /// <summary>
        /// 从配置中获取数据库提供程序类型
        /// 读取配置文件中的设置，确定使用哪种数据库
        /// 如果配置不存在，默认使用SQL Server
        /// </summary>
        /// <returns>数据库提供程序类型枚举</returns>
        private DatabaseProviderType GetProviderTypeFromConfig()
        {
            var providerName = _configuration["Database:ProviderType"] ?? "SqlServer";
            
            return providerName.ToLowerInvariant() switch
            {
                "sqlserver" => DatabaseProviderType.SqlServer,
                "postgresql" => DatabaseProviderType.PostgreSQL,
                "mysql" => DatabaseProviderType.MySQL,
                "sqlite" => DatabaseProviderType.SQLite,
                "inmemory" => DatabaseProviderType.InMemory,
                _ => DatabaseProviderType.SqlServer
            };
        }

        /// <summary>
        /// 获取连接字符串
        /// 直接使用DefaultConnection连接字符串
        /// 简化配置，不再需要为每种数据库类型配置单独的连接字符串
        /// </summary>
        /// <returns>数据库连接字符串</returns>
        private string GetConnectionString()
        {
            // 直接使用DefaultConnection连接字符串
            return _configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }
    }
} 