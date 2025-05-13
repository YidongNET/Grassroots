using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Grassroots.Application.Commands;
using Grassroots.Application.Events;
using Grassroots.Application.Queries;
using Grassroots.Domain.Events;

namespace Grassroots.Infrastructure.DependencyInjection.AutofacModules
{
    public class ApplicationModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var applicationAssembly = Assembly.Load("Grassroots.Application");
            var infrastructureAssembly = Assembly.Load("Grassroots.Infrastructure");

            // 注册命令处理器
            RegisterGenericHandlers(builder, typeof(ICommandHandler<>), applicationAssembly, infrastructureAssembly);
            RegisterGenericHandlers(builder, typeof(ICommandHandler<,>), applicationAssembly, infrastructureAssembly);

            // 注册查询处理器
            RegisterGenericHandlers(builder, typeof(IQueryHandler<,>), applicationAssembly, infrastructureAssembly);

            // 注册事件处理器
            RegisterGenericHandlers(builder, typeof(IDomainEventHandler<>), applicationAssembly, infrastructureAssembly);
            RegisterGenericHandlers(builder, typeof(IIntegrationEventHandler<>), applicationAssembly, infrastructureAssembly);
        }

        private void RegisterGenericHandlers(ContainerBuilder builder, Type handlerType, params Assembly[] assemblies)
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
                        builder.RegisterType(type).As(interfaceType).InstancePerLifetimeScope();
                    }
                }
            }
        }
    }
} 