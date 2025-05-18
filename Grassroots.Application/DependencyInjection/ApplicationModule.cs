using Autofac;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Grassroots.Application.DependencyInjection;

/// <summary>
/// 应用层Autofac模块
/// </summary>
public class ApplicationModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // 注册MediatR
        var applicationAssembly = Assembly.GetExecutingAssembly();
        var domainAssembly = typeof(Domain.Events.IDomainEvent).Assembly;
        
        // 注册所有处理器
        builder.RegisterAssemblyTypes(applicationAssembly, domainAssembly)
            .AsClosedTypesOf(typeof(IRequestHandler<,>))
            .AsImplementedInterfaces();
        
        builder.RegisterAssemblyTypes(applicationAssembly, domainAssembly)
            .AsClosedTypesOf(typeof(INotificationHandler<>))
            .AsImplementedInterfaces();
        
        // 注册前置处理器和后置处理器
        builder.RegisterGeneric(typeof(RequestPreProcessorBehavior<,>)).AsImplementedInterfaces();
        builder.RegisterGeneric(typeof(RequestPostProcessorBehavior<,>)).AsImplementedInterfaces();
        
        // 注册MediatR
        builder.RegisterType<Mediator>().As<IMediator>().InstancePerLifetimeScope();
        
        // 注册处理器访问器
        builder.Register<Func<Type, object>>(context =>
        {
            var c = context.Resolve<IComponentContext>();
            return t => c.Resolve(t);
        }).InstancePerLifetimeScope();
    }
} 