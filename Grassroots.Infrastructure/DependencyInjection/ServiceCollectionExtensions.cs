using System;
using System.Reflection;
using Grassroots.Application.Commands;
using Grassroots.Application.Dispatchers;
using Grassroots.Application.Events;
using Grassroots.Application.Queries;
using Grassroots.Domain.Events;
using Grassroots.Domain.Repositories;
using Grassroots.Infrastructure.Commands;
using Grassroots.Infrastructure.Data;
using Grassroots.Infrastructure.Events;
using Grassroots.Infrastructure.Mapping;
using Grassroots.Infrastructure.Queries;
using Grassroots.Infrastructure.ServiceDiscovery;
using Grassroots.Model.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IMapperInterface = Grassroots.Model.Mapping.IMapper;

namespace Grassroots.Infrastructure.DependencyInjection
{
    /// <summary>
    /// 服务集合扩展
    /// </summary>
    public static class ServiceCollectionExtensions
    {        
        /// <summary>
        /// 添加基础设施服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configuration">配置</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // 注册命令和查询分发器
            services.AddScoped<ICommandDispatcher, CommandDispatcher>();
            services.AddScoped<IQueryDispatcher, QueryDispatcher>();

            // 注册AutoMapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddSingleton<IMapperInterface, AutoMapperAdapter>();

            // 注册事件相关服务
            services.AddSingleton<IDomainEventBus, DomainEventBus>();
            services.AddSingleton<IIntegrationEventBus, IntegrationEventBus>();
            services.AddScoped<IEventStore, EventStore>();
            services.AddScoped<IEventMediator, EventMediator>();

            // 注册事件处理器
            RegisterEventHandlers(services);

            return services;
        }

        /// <summary>
        /// 添加应用层服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // 注册命令处理器
            var commandHandlerType = typeof(ICommandHandler<>);
            var commandWithResultHandlerType = typeof(ICommandHandler<,>);
            
            RegisterGenericTypes(services, commandHandlerType);
            RegisterGenericTypes(services, commandWithResultHandlerType);

            // 注册查询处理器
            var queryHandlerType = typeof(IQueryHandler<,>);
            RegisterGenericTypes(services, queryHandlerType);

            return services;
        }

        /// <summary>
        /// 注册泛型类型
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="serviceType">服务类型</param>
        private static void RegisterGenericTypes(IServiceCollection services, System.Type serviceType)
        {
            var assemblies = new[] 
            { 
                Assembly.GetExecutingAssembly(), 
                Assembly.Load("Grassroots.Application")
            };

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes()
                    .Where(t => !t.IsAbstract && !t.IsInterface &&
                        t.GetInterfaces().Any(i => i.IsGenericType && 
                        i.GetGenericTypeDefinition() == serviceType));

                foreach (var type in types)
                {
                    var interfaceType = type.GetInterfaces()
                        .First(i => i.IsGenericType && 
                        i.GetGenericTypeDefinition() == serviceType);

                    services.AddScoped(interfaceType, type);
                }
            }
        }

        /// <summary>
        /// 注册事件处理器
        /// </summary>
        /// <param name="services">服务集合</param>
        private static void RegisterEventHandlers(IServiceCollection services)
        {
            var assemblies = new[] 
            { 
                Assembly.GetExecutingAssembly(), 
                Assembly.Load("Grassroots.Application")
            };

            // 注册领域事件处理器
            var domainEventHandlerType = typeof(IDomainEventHandler<>);
            RegisterEventHandlerType(services, domainEventHandlerType, assemblies);

            // 注册集成事件处理器
            var integrationEventHandlerType = typeof(IIntegrationEventHandler<>);
            RegisterEventHandlerType(services, integrationEventHandlerType, assemblies);

            // 配置事件总线订阅
            var sp = services.BuildServiceProvider();
            var domainEventBus = sp.GetRequiredService<IDomainEventBus>();
            var integrationEventBus = sp.GetRequiredService<IIntegrationEventBus>();

            ConfigureEventBusSubscriptions(domainEventBus, integrationEventBus, assemblies);
        }

        /// <summary>
        /// 注册事件处理器类型
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="handlerType">处理器类型</param>
        /// <param name="assemblies">程序集</param>
        private static void RegisterEventHandlerType(IServiceCollection services, Type handlerType, Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes()
                    .Where(t => !t.IsAbstract && !t.IsInterface &&
                        t.GetInterfaces().Any(i => i.IsGenericType && 
                        i.GetGenericTypeDefinition() == handlerType));

                foreach (var type in types)
                {
                    var interfaces = type.GetInterfaces()
                        .Where(i => i.IsGenericType && 
                        i.GetGenericTypeDefinition() == handlerType);

                    foreach (var interfaceType in interfaces)
                    {
                        services.AddScoped(interfaceType, type);
                    }
                }
            }
        }

        /// <summary>
        /// 配置事件总线订阅
        /// </summary>
        /// <param name="domainEventBus">领域事件总线</param>
        /// <param name="integrationEventBus">集成事件总线</param>
        /// <param name="assemblies">程序集</param>
        private static void ConfigureEventBusSubscriptions(IDomainEventBus domainEventBus, IIntegrationEventBus integrationEventBus, Assembly[] assemblies)
        {
            // 注册领域事件处理器
            var domainEventHandlerType = typeof(IDomainEventHandler<>);
            foreach (var assembly in assemblies)
            {
                var domainEventTypes = assembly.GetTypes()
                    .Where(t => !t.IsAbstract && !t.IsInterface &&
                                t.GetInterfaces().Any(i => i.IsGenericType && 
                                                         i.GetGenericTypeDefinition() == domainEventHandlerType));

                foreach (var handlerType in domainEventTypes)
                {
                    var interfaces = handlerType.GetInterfaces()
                        .Where(i => i.IsGenericType && 
                                    i.GetGenericTypeDefinition() == domainEventHandlerType);

                    foreach (var interfaceType in interfaces)
                    {
                        var eventType = interfaceType.GetGenericArguments()[0];
                        var registerMethod = domainEventBus.GetType().GetMethod("Register");
                        var genericRegisterMethod = registerMethod.MakeGenericMethod(eventType);
                        genericRegisterMethod.Invoke(domainEventBus, null);
                    }
                }
            }

            // 注册集成事件处理器
            var integrationEventHandlerType = typeof(IIntegrationEventHandler<>);
            foreach (var assembly in assemblies)
            {
                var integrationEventTypes = assembly.GetTypes()
                    .Where(t => !t.IsAbstract && !t.IsInterface &&
                                t.GetInterfaces().Any(i => i.IsGenericType && 
                                                         i.GetGenericTypeDefinition() == integrationEventHandlerType));

                foreach (var handlerType in integrationEventTypes)
                {
                    var interfaces = handlerType.GetInterfaces()
                        .Where(i => i.IsGenericType && 
                                    i.GetGenericTypeDefinition() == integrationEventHandlerType);

                    foreach (var interfaceType in interfaces)
                    {
                        var eventType = interfaceType.GetGenericArguments()[0];
                        var registerMethod = integrationEventBus.GetType().GetMethod("Register");
                        var genericRegisterMethod = registerMethod.MakeGenericMethod(eventType);
                        genericRegisterMethod.Invoke(integrationEventBus, null);
                    }
                }
            }
        }
    }
} 