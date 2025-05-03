using System;
using Grassroots.Domain.Repositories;
using Grassroots.Infrastructure.Data;
using Grassroots.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Grassroots.Infrastructure.Extensions
{
    /// <summary>
    /// 数据库服务注册扩展
    /// </summary>
    public static class DatabaseServiceCollectionExtensions
    {
        /// <summary>
        /// 添加数据库服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configuration">配置</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
        {
            // 获取连接字符串
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("数据库连接字符串未配置。请在appsettings.json中配置ConnectionStrings:DefaultConnection");
            }

            // 获取数据库提供程序类型
            var providerType = configuration.GetValue<string>("Database:ProviderType") ?? "SqlServer";

            // 添加DbContext
            services.AddDbContext<AppDbContext>(options =>
            {
                AppDbContextFactory.ConfigureDbProvider(options, providerType, connectionString);
            });

            // 注册仓储
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            return services;
        }
    }
} 