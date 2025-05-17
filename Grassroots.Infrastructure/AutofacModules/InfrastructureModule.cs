using Autofac;
using Grassroots.Application.Common.Interfaces;
using Grassroots.Domain.Repositories;
using Grassroots.Infrastructure.Persistence;
using Grassroots.Infrastructure.Repositories;
using Grassroots.Infrastructure.ServiceDiscovery;
using Grassroots.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;

namespace Grassroots.Infrastructure.AutofacModules
{
    public class InfrastructureModule : Module
    {
        private readonly IConfiguration _configuration;

        public InfrastructureModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            // 注册数据库工厂
            builder.RegisterType<DatabaseFactory>()
                .AsSelf()
                .SingleInstance();

            // 注册DbContext
            builder.Register(c =>
            {
                var dbFactory = c.Resolve<DatabaseFactory>();
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                
                // 使用工厂配置DbContext
                dbFactory.ConfigureDbContext(optionsBuilder);

                return new ApplicationDbContext(optionsBuilder.Options);
            })
            .As<ApplicationDbContext>()
            .As<IApplicationDbContext>()
            .InstancePerLifetimeScope();

            // 注册仓储
            builder.RegisterGeneric(typeof(Repository<>))
                .As(typeof(IRepository<>))
                .InstancePerLifetimeScope();
                
            // 注册雪花算法配置
            builder.Register(c => 
            {
                var options = new SnowflakeOptions();
                _configuration.GetSection("Snowflake").Bind(options);
                return Options.Create(options);
            })
            .AsImplementedInterfaces()
            .SingleInstance();
            
            // 注册雪花算法ID生成器
            builder.RegisterType<SnowflakeIdGenerator>()
                .As<IIdGenerator>()
                .SingleInstance();
                
            // 注册Consul配置
            builder.Register(c => 
            {
                var options = new ConsulOptions();
                _configuration.GetSection("Consul").Bind(options);
                return Options.Create(options);
            })
            .AsImplementedInterfaces()
            .SingleInstance();
            
            // 注册Consul服务发现
            builder.RegisterType<ConsulServiceDiscovery>()
                .As<IServiceDiscovery>()
                .SingleInstance();
        }
    }
} 