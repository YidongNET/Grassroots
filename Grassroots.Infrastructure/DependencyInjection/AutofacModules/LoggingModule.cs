using Autofac;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace Grassroots.Infrastructure.DependencyInjection.AutofacModules
{
    /// <summary>
    /// 日志模块
    /// </summary>
    /// <remarks>
    /// 使用Serilog实现的日志模块，负责配置和注册日志服务
    /// </remarks>
    public class LoggingModule : Module
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// 初始化日志模块
        /// </summary>
        /// <param name="configuration">配置</param>
        public LoggingModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 加载模块
        /// </summary>
        /// <param name="builder">容器构建器</param>
        protected override void Load(ContainerBuilder builder)
        {
            // 注册Serilog日志服务
            // 此模块使用Autofac注册，但实际日志配置在Program.cs中完成

            // 如果有需要注册的其他日志相关服务，可以在此处添加
        }
    }
} 