using Autofac;
using Grassroots.Infrastructure.DependencyInjection.AutofacModules;
using Microsoft.Extensions.Configuration;

namespace Grassroots.Infrastructure.DependencyInjection
{
    /// <summary>
    /// Autofac扩展类
    /// </summary>
    public static class AutofacExtensions
    {
        /// <summary>
        /// 注册所有Autofac模块
        /// </summary>
        /// <param name="builder">容器构建器</param>
        /// <param name="configuration">配置</param>
        public static void RegisterGrassrootsModules(this ContainerBuilder builder, IConfiguration configuration)
        {
            // 注册基础设施模块
            builder.RegisterModule(new InfrastructureModule(configuration));
            
            // 注册应用层模块
            builder.RegisterModule(new ApplicationModule());
            
            // 注册数据库模块
            builder.RegisterModule(new DbModule(configuration));
            
            // 注册服务发现模块
            builder.RegisterModule(new ServiceDiscoveryModule(configuration));
        }
    }
} 