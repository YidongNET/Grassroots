using Autofac;
using Grassroots.Infrastructure.DependencyInjection.AutofacModules;
using Microsoft.Extensions.Configuration;

namespace Grassroots.Infrastructure.DependencyInjection
{
    /// <summary>
    /// Autofac扩展类
    /// </summary>
    /// <remarks>
    /// 本类用于集中管理Autofac模块的注册，实现依赖注入容器的模块化配置。
    /// 通过此扩展类，可以将各个功能模块的依赖注入配置分离到不同的模块类中，
    /// 使代码结构更清晰，更易于维护。
    /// </remarks>
    public static class AutofacExtensions
    {
        /// <summary>
        /// 注册所有Autofac模块
        /// </summary>
        /// <param name="builder">容器构建器</param>
        /// <param name="configuration">配置</param>
        /// <remarks>
        /// 此方法按照依赖关系顺序注册各个模块:
        /// 1. LoggingModule - 注册日志服务
        /// 2. InfrastructureModule - 注册基础设施服务，如命令分发器、事件总线等
        /// 3. ApplicationModule - 注册应用层服务，如命令和查询处理器
        /// 4. DbModule - 注册数据库相关服务，如DbContext和仓储实现
        /// 5. ServiceDiscoveryModule - 注册服务发现相关服务，如Consul客户端
        /// 
        /// 每个模块负责注册特定领域的服务，这种模块化方式使依赖注入配置更加清晰和可维护。
        /// </remarks>
        public static void RegisterGrassrootsModules(this ContainerBuilder builder, IConfiguration configuration)
        {
            // 注册日志模块 - 包含Serilog日志服务
            builder.RegisterModule(new LoggingModule(configuration));
            
            // 注册基础设施模块 - 包含基础组件如命令/查询分发器、事件总线等
            builder.RegisterModule(new InfrastructureModule(configuration));
            
            // 注册应用层模块 - 包含应用层服务如命令和查询处理器
            builder.RegisterModule(new ApplicationModule());
            
            // 注册数据库模块 - 包含DbContext、单元工作和特定仓储实现
            builder.RegisterModule(new DbModule(configuration));
            
            // 注册服务发现模块 - 包含服务发现和注册相关组件
            builder.RegisterModule(new ServiceDiscoveryModule(configuration));
        }
    }
} 