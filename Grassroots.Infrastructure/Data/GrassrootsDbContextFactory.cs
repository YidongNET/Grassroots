using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Grassroots.Infrastructure.Data
{
    /// <summary>
    /// 数据库上下文工厂，用于EF Core迁移
    /// </summary>
    public class GrassrootsDbContextFactory : IDesignTimeDbContextFactory<GrassrootsDbContext>
    {
        public GrassrootsDbContext CreateDbContext(string[] args)
        {
            // 从appsettings.json获取配置
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            // 获取数据库连接字符串
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            
            // 获取数据库提供程序类型
            var dbProviderType = configuration.GetValue<string>("Database:ProviderType") ?? "SqlServer";

            // 创建DbContext选项构建器
            var optionsBuilder = new DbContextOptionsBuilder<GrassrootsDbContext>();
            
            // 根据数据库提供程序类型配置选项
            ConfigureDbProvider(optionsBuilder, dbProviderType, connectionString);

            return new GrassrootsDbContext(optionsBuilder.Options);
        }

        /// <summary>
        /// 根据数据库提供程序类型配置DbContext选项
        /// </summary>
        /// <param name="optionsBuilder">DbContext选项构建器</param>
        /// <param name="providerType">数据库提供程序类型</param>
        /// <param name="connectionString">连接字符串</param>
        public static void ConfigureDbProvider(DbContextOptionsBuilder optionsBuilder, string providerType, string connectionString)
        {
            switch (providerType.ToLower())
            {
                case "sqlserver":
                    optionsBuilder.UseSqlServer(connectionString,
                        sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly("Grassroots.Infrastructure");
                            sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                        });
                    break;
                case "postgresql":
                    optionsBuilder.UseNpgsql(connectionString,
                        npgsqlOptions =>
                        {
                            npgsqlOptions.MigrationsAssembly("Grassroots.Infrastructure");
                            npgsqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                        });
                    break;
                case "mysql":
                    optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
                        mySqlOptions =>
                        {
                            mySqlOptions.MigrationsAssembly("Grassroots.Infrastructure");
                            mySqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                        });
                    break;
                default:
                    throw new ArgumentException($"不支持的数据库提供程序类型: {providerType}", nameof(providerType));
            }
        }
    }
} 