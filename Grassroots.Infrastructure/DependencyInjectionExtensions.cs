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
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// 添加基础设施服务
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
    /// </summary>
    public static IServiceCollection ConfigureDbContext(
        this IServiceCollection services,
        IConfiguration configuration,
        DatabaseType databaseType,
        string connectionStringName)
    {
        var connectionString = configuration.GetConnectionString(connectionStringName);
        
        // 根据数据库类型配置DbContext
        services.AddDbContext<GrassrootsDbContext>(options =>
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
        services.AddDbContext<GrassrootsDbContext>();
        
        return services;
    }
    
    /// <summary>
    /// 添加ID生成器
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