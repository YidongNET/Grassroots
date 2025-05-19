using Autofac;
using Grassroots.Domain.Events;
using Grassroots.Domain.Interfaces;
using Grassroots.Infrastructure.Data;
using Grassroots.Infrastructure.Events;
using Grassroots.Infrastructure.IdGenerators;
using Grassroots.Infrastructure.ServiceDiscovery;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace Grassroots.Infrastructure;

/// <summary>
/// 基础设施层依赖注入扩展
/// 提供将基础设施层服务注册到DI容器的扩展方法
/// 包括数据库上下文、仓储、工作单元、事件服务等基础组件的注册
/// 支持不同数据库提供程序的配置和切换
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// 添加基础设施服务
    /// 注册所有基础设施层组件到标准.NET依赖注入容器
    /// 作为应用程序启动时的主要注册入口
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configuration">配置</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 获取数据库类型配置
        var databaseSection = configuration.GetSection("Database");
        var databaseTypeStr = databaseSection["ProviderType"] ?? "SqlServer";
        
        if (!Enum.TryParse<DatabaseType>(databaseTypeStr, true, out var databaseType))
        {
            databaseType = DatabaseType.SqlServer; // 默认使用SQL Server
        }

        // 获取连接字符串，使用默认连接字符串
        string connectionStringName = "DefaultConnection";

        // 配置DbContext
        services.ConfigureDbContext(configuration, databaseType, connectionStringName);

        // 注册Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // 注册泛型仓储
        services.AddScoped(typeof(IRepository<>), typeof(RepositoryBase<>));
        
        // 注册事件相关服务
        services.AddEventServices();
        
        // 注册ID生成器
        services.AddIdGenerators(configuration);
        
        // 注册服务发现
        services.AddServiceDiscovery(configuration);

        return services;
    }
    
    /// <summary>
    /// 配置DbContext
    /// 根据配置选择不同的数据库提供程序
    /// 支持SQL Server、PostgreSQL和MySQL三种数据库
    /// 设置每种数据库特定的选项和迁移程序集
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configuration">配置</param>
    /// <param name="databaseType">数据库类型</param>
    /// <param name="connectionStringName">连接字符串名称</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection ConfigureDbContext(
        this IServiceCollection services,
        IConfiguration configuration,
        DatabaseType databaseType,
        string connectionStringName)
    {
        var connectionString = configuration.GetConnectionString(connectionStringName);
        
        // 根据数据库类型配置DbContext
        services.AddDbContext<AppDbContext>(options =>
        {
            switch (databaseType)
            {
                case DatabaseType.SqlServer:
                    options.UseSqlServer(connectionString, 
                        sqlOptions => sqlOptions.MigrationsAssembly("Grassroots.Infrastructure"));
                    break;
                case DatabaseType.PostgreSQL:
                    options.UseNpgsql(connectionString,
                        npgsqlOptions => npgsqlOptions.MigrationsAssembly("Grassroots.Infrastructure"));
                    break;
                case DatabaseType.MySQL:
                    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
                        mysqlOptions => mysqlOptions.MigrationsAssembly("Grassroots.Infrastructure"));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(databaseType), "不支持的数据库类型");
            }
        });
        
        return services;
    }
    
    /// <summary>
    /// 添加事件相关服务
    /// 注册领域事件服务、事件溯源存储、事件总线等
    /// 配置事件处理和分发的基础设施
    /// 建立领域事件和集成事件之间的映射关系
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    private static IServiceCollection AddEventServices(this IServiceCollection services)
    {
        // 注册领域事件服务
        services.AddScoped<IDomainEventService, DomainEventService>();
        
        // 注册事件溯源存储
        services.AddScoped<IEventStore, EventStore>();
        
        // 注册事件总线
        services.AddSingleton<IEventBus, InMemoryEventBus>();
        
        // 注册领域事件到集成事件的映射器
        services.AddSingleton<IDomainToIntegrationEventMapper, DomainToIntegrationEventMapper>();
        
        // 注册泛型领域事件处理器
        services.AddScoped(typeof(IDomainEventHandler<>), typeof(DomainEventToIntegrationEventHandler<>));
        
        // 配置事件表
        services.AddDbContext<AppDbContext>();
        
        return services;
    }
    
    /// <summary>
    /// 添加ID生成器
    /// 配置和注册雪花算法(Snowflake)ID生成器
    /// 用于生成分布式环境下的唯一ID
    /// 通过配置文件设置工作节点ID、数据中心ID等参数
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configuration">配置</param>
    /// <returns>服务集合</returns>
    private static IServiceCollection AddIdGenerators(this IServiceCollection services, IConfiguration configuration)
    {
        // 配置雪花算法选项
        services.Configure<SnowflakeOptions>(options =>
        {
            var section = configuration.GetSection("Snowflake");
            
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
            
            options.DatacenterId = datacenterId;
            options.WorkerId = workerId;
            options.Epoch = epoch;
            options.SequenceBits = sequenceBits;
            options.WorkerIdBits = workerIdBits;
            options.DatacenterIdBits = datacenterIdBits;
        });
        
        // 注册雪花算法ID生成器
        services.AddSingleton<IIdGenerator, SnowflakeIdGenerator>();
        
        return services;
    }
    
    /// <summary>
    /// 添加服务发现
    /// 配置Consul服务注册与发现功能
    /// 允许应用在微服务架构中自动注册和发现服务
    /// 通过配置文件设置服务名称、地址、健康检查等参数
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configuration">配置</param>
    /// <returns>服务集合</returns>
    private static IServiceCollection AddServiceDiscovery(this IServiceCollection services, IConfiguration configuration)
    {
        // 配置Consul选项
        services.Configure<ConsulOptions>(options =>
        {
            var section = configuration.GetSection("Consul");
            
            if (section.Exists())
            {
                if (bool.TryParse(section["Enabled"], out bool enabled))
                {
                    options.Enabled = enabled;
                }
                
                if (!string.IsNullOrEmpty(section["Address"]))
                {
                    options.Address = section["Address"];
                }
                
                if (!string.IsNullOrEmpty(section["ServiceId"]))
                {
                    options.ServiceId = section["ServiceId"];
                }
                
                if (!string.IsNullOrEmpty(section["ServiceName"]))
                {
                    options.ServiceName = section["ServiceName"];
                }
                
                if (!string.IsNullOrEmpty(section["ServiceAddress"]))
                {
                    options.ServiceAddress = section["ServiceAddress"];
                }
                
                if (int.TryParse(section["ServicePort"], out int servicePort))
                {
                    options.ServicePort = servicePort;
                }
                
                if (!string.IsNullOrEmpty(section["HealthCheck"]))
                {
                    options.HealthCheck = section["HealthCheck"];
                }
                
                if (int.TryParse(section["HealthCheckInterval"], out int healthCheckInterval))
                {
                    options.HealthCheckInterval = healthCheckInterval;
                }
                
                if (int.TryParse(section["HealthCheckTimeout"], out int healthCheckTimeout))
                {
                    options.HealthCheckTimeout = healthCheckTimeout;
                }
                
                if (bool.TryParse(section["DeregisterCriticalServiceAfter"], out bool deregisterCriticalServiceAfter))
                {
                    options.DeregisterCriticalServiceAfter = deregisterCriticalServiceAfter;
                }
                
                if (int.TryParse(section["DeregisterCriticalServiceAfterMinutes"], out int deregisterCriticalServiceAfterMinutes))
                {
                    options.DeregisterCriticalServiceAfterMinutes = deregisterCriticalServiceAfterMinutes;
                }
                
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
                    
                    if (tags.Count > 0)
                    {
                        options.Tags = tags.ToArray();
                    }
                }
            }
        });
        
        // 注册Consul服务发现
        services.AddSingleton<IServiceDiscovery, ConsulServiceDiscovery>();
        
        return services;
    }
} 