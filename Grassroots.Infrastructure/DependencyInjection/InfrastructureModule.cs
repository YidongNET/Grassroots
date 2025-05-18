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
/// </summary>
public class InfrastructureModule : Autofac.Module
{
    private readonly IConfiguration _configuration;

    public InfrastructureModule(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void Load(ContainerBuilder builder)
    {
        // 注册日志服务
        builder.RegisterInstance(Log.Logger).As<Serilog.ILogger>().SingleInstance();

        // 注册Unit of Work
        builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();

        // 注册泛型仓储
        builder.RegisterGeneric(typeof(RepositoryBase<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();

        // 注册事件相关服务
        RegisterEventServices(builder);
        
        // 注册ID生成器
        RegisterIdGenerators(builder);
        
        // 注册服务发现
        RegisterServiceDiscovery(builder);
    }

    private void RegisterEventServices(ContainerBuilder builder)
    {
        // 注册领域事件服务
        builder.RegisterType<DomainEventService>().As<IDomainEventService>().InstancePerLifetimeScope();
        
        // 注册事件溯源存储
        builder.RegisterType<EventStore>().As<IEventStore>().InstancePerLifetimeScope();
        
        // 注册事件总线
        builder.RegisterType<InMemoryEventBus>().As<IEventBus>().SingleInstance();
        
        // 注册领域事件到集成事件的映射器
        builder.RegisterType<DomainToIntegrationEventMapper>().As<IDomainToIntegrationEventMapper>().SingleInstance();
        
        // 注册泛型领域事件处理器
        builder.RegisterGeneric(typeof(DomainEventToIntegrationEventHandler<>))
            .As(typeof(IDomainEventHandler<>))
            .InstancePerLifetimeScope();
    }
    
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
        
        builder.RegisterInstance(Options.Create(snowflakeOptions))
            .As<IOptions<SnowflakeOptions>>()
            .SingleInstance();
            
        // 注册雪花算法ID生成器
        builder.RegisterType<SnowflakeIdGenerator>()
            .As<IIdGenerator>()
            .SingleInstance();
    }
    
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
                
                if (tags.Count > 0)
                {
                    consulOptions.Tags = tags.ToArray();
                }
            }
        }
        
        builder.RegisterInstance(Options.Create(consulOptions))
            .As<IOptions<ConsulOptions>>()
            .SingleInstance();
        
        // 注册Consul服务发现
        builder.RegisterType<ConsulServiceDiscovery>()
            .As<IServiceDiscovery>()
            .SingleInstance();
    }
} 