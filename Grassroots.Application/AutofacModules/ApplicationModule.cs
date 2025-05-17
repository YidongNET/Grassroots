using Autofac;
using Grassroots.Application.Common.Behaviors;
using MediatR;
using System.Reflection;
using Module = Autofac.Module;

namespace Grassroots.Application.AutofacModules
{
    public class ApplicationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // 注册MediatR
            builder.RegisterAssemblyTypes(typeof(ApplicationModule).Assembly)
                .AsClosedTypesOf(typeof(IRequestHandler<,>))
                .InstancePerLifetimeScope();

            // 注册MediatR behaviors
            builder.RegisterGeneric(typeof(ValidationBehavior<,>))
                .As(typeof(IPipelineBehavior<,>))
                .InstancePerLifetimeScope();
        }
    }
} 