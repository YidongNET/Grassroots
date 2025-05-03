using System.Reflection;
using Grassroots.Application.Commands;
using Grassroots.Application.Dispatchers;
using Grassroots.Application.Queries;
using Grassroots.Domain.Repositories;
using Grassroots.Infrastructure.Commands;
using Grassroots.Infrastructure.Data;
using Grassroots.Infrastructure.Mapping;
using Grassroots.Infrastructure.Queries;
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
    }
} 