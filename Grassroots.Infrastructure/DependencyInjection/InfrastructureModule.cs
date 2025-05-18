using Autofac;
using Grassroots.Domain.Events;
using Grassroots.Domain.Interfaces;
using Grassroots.Infrastructure.Data;
using Grassroots.Infrastructure.Events;
using Grassroots.Infrastructure.IdGenerators;
using Grassroots.Infrastructure.ServiceDiscovery;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Serilog;
using System.Collections.Generic;
using System.Reflection;

namespace Grassroots.Infrastructure.DependencyInjection;

/// <summary>
/// 基础设施层Autofac模块
/// 使用Autofac模块化方式注册基础设施层的服务和组件
/// 相比标准DI容器，提供更灵活的注册方式和更多的生命周期选项
/// 集中管理基础设施层的依赖关系，便于维护和扩展
/// </summary>
public class InfrastructureModule : Autofac.Module
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="configuration">配置对象，用于获取各服务组件的配置参数</param>
    public InfrastructureModule(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// 加载模块
    /// Autofac模块的核心方法，在此注册所有基础设施层服务
    /// 按照功能分组注册各类服务，如仓储、事件、ID生成器等
    /// </summary>
    /// <param name="builder">Autofac容器构建器</param>
    protected override void Load(ContainerBuilder builder)
    {
        // 注册日志服务
        // 使用Serilog作为日志提供程序，单例模式确保全局共享同一日志实例
        builder.RegisterInstance(Log.Logger).As<Serilog.ILogger>().SingleInstance();

        // 注册Unit of Work
        // 作用域生命周期确保在同一请求中使用相同的工作单元实例
        // 负责协调多个仓储的事务一致性
        builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();

        // 注册泛型仓储
        // 支持所有实体类型的统一数据访问接口
        // 作用域生命周期确保在同一请求中共享同一实例，维持数据一致性
        builder.RegisterGeneric(typeof(RepositoryBase<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();

        // 注册事件相关服务
        RegisterEventServices(builder);
        
        // 注册ID生成器
        RegisterIdGenerators(builder);
        
        // 注册服务发现
        RegisterServiceDiscovery(builder);
    }

    /// <summary>
    /// 注册事件相关服务
    /// 包括领域事件服务、事件存储、事件总线和事件处理器等
    /// 实现事件驱动架构和事件溯源模式的基础设施支持
    /// </summary>
    /// <param name="builder">Autofac容器构建器</param>
    private void RegisterEventServices(ContainerBuilder builder)
    {
        // 注册领域事件服务
        // 负责领域事件的发布和分发
        // 作用域生命周期保证在同一请求中使用相同实例
        builder.RegisterType<DomainEventService>().As<IDomainEventService>().InstancePerLifetimeScope();
        
        // 注册事件溯源存储
        // 用于持久化和检索事件历史
        // 作用域生命周期保证在同一请求中使用相同实例
        builder.RegisterType<EventStore>().As<IEventStore>().InstancePerLifetimeScope();
        
        // 注册事件总线
        // 用于跨边界（如微服务间）发布和订阅集成事件
        // 单例模式确保全局共享同一事件总线
        builder.RegisterType<InMemoryEventBus>().As<IEventBus>().SingleInstance();
        
        // 注册领域事件到集成事件的映射器
        // 负责将领域事件转换为集成事件，用于跨边界通信
        // 单例模式确保映射规则一致
        builder.RegisterType<DomainToIntegrationEventMapper>().As<IDomainToIntegrationEventMapper>().SingleInstance();
        
        // 注册泛型领域事件处理器
        // 将领域事件转换为集成事件并发布到事件总线
        // 作用域生命周期保证在同一请求中使用相同实例
        builder.RegisterGeneric(typeof(DomainEventToIntegrationEventHandler<>))
            .As(typeof(IDomainEventHandler<>))
            .InstancePerLifetimeScope();
    }
    
    /// <summary>
    /// 注册ID生成器
    /// 配置和注册分布式ID生成器，如雪花算法
    /// 用于在分布式系统中生成全局唯一的ID
    /// </summary>
    /// <param name="builder">Autofac容器构建器</param>
    private void RegisterIdGenerators(ContainerBuilder builder)
    {
        // 注册并配置雪花算法选项
        var snowflakeOptions = new SnowflakeOptions();
        var section = _configuration.GetSection("Snowflake");
        
        // 手动解析配置值
        int datacenterId = 1;
        if (int.TryParse(section["DatacenterId"], out var parsedDatacenterId))
        {
            datacenterId = parsedDatacenterId;
        }
        
        int workerId = 1;
        if (int.TryParse(section["WorkerId"], out var parsedWorkerId))
        {
            workerId = parsedWorkerId;
        }
        
        long epoch = 1672531200000;
        if (long.TryParse(section["Epoch"], out var parsedEpoch))
        {
            epoch = parsedEpoch;
        }
        
        int sequenceBits = 12;
        if (int.TryParse(section["SequenceBits"], out var parsedSequenceBits))
        {
            sequenceBits = parsedSequenceBits;
        }
        
        int workerIdBits = 5;
        if (int.TryParse(section["WorkerIdBits"], out var parsedWorkerIdBits))
        {
            workerIdBits = parsedWorkerIdBits;
        }
        
        int datacenterIdBits = 5;
        if (int.TryParse(section["DatacenterIdBits"], out var parsedDatacenterIdBits))
        {
            datacenterIdBits = parsedDatacenterIdBits;
        }
        
        snowflakeOptions.DatacenterId = datacenterId;
        snowflakeOptions.WorkerId = workerId;
        snowflakeOptions.Epoch = epoch;
        snowflakeOptions.SequenceBits = sequenceBits;
        snowflakeOptions.WorkerIdBits = workerIdBits;
        snowflakeOptions.DatacenterIdBits = datacenterIdBits;
        
        // 注册雪花算法选项
        // 单例模式确保全局共享同一配置
        builder.RegisterInstance(Options.Create(snowflakeOptions))
            .As<IOptions<SnowflakeOptions>>()
            .SingleInstance();
            
        // 注册雪花算法ID生成器
        // 单例模式确保全局唯一，避免ID冲突
        builder.RegisterType<SnowflakeIdGenerator>()
            .As<IIdGenerator>()
            .SingleInstance();
    }
    
    /// <summary>
    /// 注册服务发现
    /// 配置和注册服务发现组件，如Consul客户端
    /// 用于在微服务架构中进行服务注册和服务发现
    /// </summary>
    /// <param name="builder">Autofac容器构建器</param>
    private void RegisterServiceDiscovery(ContainerBuilder builder)
    {
        // 创建并注册Consul选项
        var consulOptions = new ConsulOptions();
        var section = _configuration.GetSection("Consul");
        
        if (section.Exists())
        {
            if (bool.TryParse(section["Enabled"], out bool enabled))
            {
                consulOptions.Enabled = enabled;
            }
            
            if (!string.IsNullOrEmpty(section["Address"]))
            {
                consulOptions.Address = section["Address"];
            }
            
            if (!string.IsNullOrEmpty(section["ServiceId"]))
            {
                consulOptions.ServiceId = section["ServiceId"];
            }
            
            if (!string.IsNullOrEmpty(section["ServiceName"]))
            {
                consulOptions.ServiceName = section["ServiceName"];
            }
            
            if (!string.IsNullOrEmpty(section["ServiceAddress"]))
            {
                consulOptions.ServiceAddress = section["ServiceAddress"];
            }
            
            if (int.TryParse(section["ServicePort"], out int servicePort))
            {
                consulOptions.ServicePort = servicePort;
            }
            
            if (!string.IsNullOrEmpty(section["HealthCheck"]))
            {
                consulOptions.HealthCheck = section["HealthCheck"];
            }
            
            if (int.TryParse(section["HealthCheckInterval"], out int healthCheckInterval))
            {
                consulOptions.HealthCheckInterval = healthCheckInterval;
            }
            
            if (int.TryParse(section["HealthCheckTimeout"], out int healthCheckTimeout))
            {
                consulOptions.HealthCheckTimeout = healthCheckTimeout;
            }
            
            if (bool.TryParse(section["DeregisterCriticalServiceAfter"], out bool deregisterCriticalServiceAfter))
            {
                consulOptions.DeregisterCriticalServiceAfter = deregisterCriticalServiceAfter;
            }
            
            if (int.TryParse(section["DeregisterCriticalServiceAfterMinutes"], out int deregisterCriticalServiceAfterMinutes))
            {
                consulOptions.DeregisterCriticalServiceAfterMinutes = deregisterCriticalServiceAfterMinutes;
            }
            
            // 配置Tags
            var tagsSection = section.GetSection("Tags");
            if (tagsSection.Exists())
            {
                var tags = new List<string>();
                foreach (var item in tagsSection.GetChildren())
                {
                    if (!string.IsNullOrEmpty(item.Value))
                    {
                        tags.Add(item.Value);
                    }
                }
                consulOptions.Tags = tags.ToArray();
            }
        }
        
        // 注册Consul选项
        // 单例模式确保全局共享同一配置
        builder.RegisterInstance(Options.Create(consulOptions))
            .As<IOptions<ConsulOptions>>()
            .SingleInstance();
            
        // 注册服务发现服务
        // 单例模式确保全局共享同一服务发现客户端
        builder.RegisterType<ConsulServiceDiscovery>()
            .As<IServiceDiscovery>()
            .SingleInstance();
    }
} 