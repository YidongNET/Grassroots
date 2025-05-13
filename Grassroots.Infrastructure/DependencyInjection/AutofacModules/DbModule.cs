using System;
using Autofac;
using Grassroots.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Grassroots.Infrastructure.DependencyInjection.AutofacModules
{
    public class DbModule : Autofac.Module
    {
        private readonly IConfiguration _configuration;

        public DbModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            // 获取连接字符串
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("数据库连接字符串未配置。请在appsettings.json中配置ConnectionStrings:DefaultConnection");
            }

            // 获取数据库提供程序类型
            var providerType = _configuration.GetValue<string>("Database:ProviderType") ?? "SqlServer";

            // 注册DbContext
            builder.Register(c =>
            {
                var options = new DbContextOptionsBuilder<AppDbContext>();
                AppDbContextFactory.ConfigureDbProvider(options, providerType, connectionString);
                return new AppDbContext(options.Options);
            })
            .AsSelf()
            .InstancePerLifetimeScope();
        }
    }
} 