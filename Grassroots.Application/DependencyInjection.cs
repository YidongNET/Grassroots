using Autofac;
using Grassroots.Application.AutofacModules;
using Grassroots.Application.Common.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Grassroots.Application
{
    public static class DependencyInjection
    {
        // 保留原方法以兼容性，但不再推荐使用
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            
            return services;
        }

        // 新方法用于Autofac注册
        public static void RegisterApplicationServices(ContainerBuilder builder)
        {
            builder.RegisterModule<ApplicationModule>();
        }
    }
} 