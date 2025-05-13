using Grassroots.Infrastructure.ServiceDiscovery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace Grassroots.Infrastructure.Extensions
{
    /// <summary>
    /// 服务发现扩展方法
    /// </summary>
    public static class ServiceDiscoveryExtensions
    {
        /// <summary>
        /// 添加服务发现
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configuration">配置</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddServiceDiscovery(this IServiceCollection services, IConfiguration configuration)
        {
            // 注册特性选项
            var featuresOptions = new FeaturesOptions();
            configuration.GetSection("Features").Bind(featuresOptions);
            services.AddSingleton(featuresOptions);
            
            // 如果服务发现功能已禁用，则仅添加健康检查
            if (!featuresOptions.ServiceDiscovery.Enabled)
            {
                // 添加健康检查
                services.AddHealthChecks();
                return services;
            }
            
            // 根据提供程序类型注册服务发现
            switch (featuresOptions.ServiceDiscovery.Provider.ToLower())
            {
                case "consul":
                    AddConsulServiceDiscovery(services, configuration);
                    break;
                // 可在此处添加其他服务发现提供程序
                default:
                    AddConsulServiceDiscovery(services, configuration);
                    break;
            }
            
            return services;
        }
        
        /// <summary>
        /// 添加Consul服务发现
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configuration">配置</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddConsulServiceDiscovery(this IServiceCollection services, IConfiguration configuration)
        {
            // 注册Consul配置
            var consulOptions = new ConsulOptions();
            configuration.GetSection("Consul").Bind(consulOptions);
            services.AddSingleton(consulOptions);

            // 注册服务发现接口
            services.AddSingleton<IServiceDiscovery, ConsulServiceDiscovery>();
            
            // 注册HTTP客户端工厂
            services.AddSingleton<ServiceDiscoveryHttpClientFactory>();

            // 添加健康检查
            services.AddHealthChecks();

            return services;
        }

        /// <summary>
        /// 使用服务注册
        /// </summary>
        /// <param name="app">应用程序构建器</param>
        /// <param name="lifetime">托管生命周期</param>
        /// <param name="configuration">配置</param>
        /// <returns>应用程序构建器</returns>
        public static IApplicationBuilder UseServiceRegistration(this IApplicationBuilder app, IHostApplicationLifetime lifetime, IConfiguration configuration)
        {
            // 获取特性选项
            var featuresOptions = app.ApplicationServices.GetService<FeaturesOptions>();
            if (featuresOptions == null || !featuresOptions.ServiceDiscovery.Enabled)
            {
                // 服务发现功能已禁用，仅添加健康检查终结点
                app.UseHealthChecks("/health");
                
                var logger = app.ApplicationServices.GetService<ILogger<IServiceDiscovery>>();
                logger?.LogInformation("服务发现功能已禁用");
                
                return app;
            }
            
            // 使用适当的服务注册
            switch (featuresOptions.ServiceDiscovery.Provider.ToLower())
            {
                case "consul":
                    var consulOptions = app.ApplicationServices.GetService<ConsulOptions>();
                    if (consulOptions != null && consulOptions.Enabled)
                    {
                        UseConsulServiceRegistration(app, lifetime, configuration);
                    }
                    else
                    {
                        app.UseHealthChecks("/health");
                        var logger = app.ApplicationServices.GetService<ILogger<IServiceDiscovery>>();
                        logger?.LogInformation("Consul服务已禁用");
                    }
                    break;
                // 可在此处添加其他服务注册提供程序
                default:
                    UseConsulServiceRegistration(app, lifetime, configuration);
                    break;
            }
            
            return app;
        }

        /// <summary>
        /// 使用Consul服务注册
        /// </summary>
        /// <param name="app">应用程序构建器</param>
        /// <param name="lifetime">托管生命周期</param>
        /// <param name="configuration">配置</param>
        /// <returns>应用程序构建器</returns>
        public static IApplicationBuilder UseConsulServiceRegistration(this IApplicationBuilder app, IHostApplicationLifetime lifetime, IConfiguration configuration)
        {
            // 获取服务ID和名称
            var serviceId = configuration["Service:Id"] ?? Guid.NewGuid().ToString();
            var serviceName = configuration["Service:Name"] ?? "GrassrootsService";

            // 获取服务地址和端口
            var serviceAddress = configuration["Service:Address"];
            if (string.IsNullOrEmpty(serviceAddress))
            {
                // 如果未配置地址，则尝试获取主机地址
                serviceAddress = configuration["Service:Host"] ?? "localhost";
            }

            // 获取服务端口
            if (!int.TryParse(configuration["Service:Port"], out int servicePort))
            {
                // 使用默认端口或获取当前使用的端口
                servicePort = 5000;
            }

            // 获取标签
            var tags = configuration.GetSection("Service:Tags").Get<string[]>() ?? Array.Empty<string>();

            // 获取服务发现服务
            var serviceDiscovery = app.ApplicationServices.GetRequiredService<IServiceDiscovery>();
            var logger = app.ApplicationServices.GetService<ILogger<IServiceDiscovery>>();
            
            // 应用启动时注册服务
            lifetime.ApplicationStarted.Register(async () =>
            {
                try
                {
                    await serviceDiscovery.RegisterServiceAsync(serviceId, serviceName, serviceAddress, servicePort, tags);
                    logger?.LogInformation("服务已在Consul中注册: {ServiceId}, {ServiceName}, {Address}:{Port}", 
                        serviceId, serviceName, serviceAddress, servicePort);
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex, "在Consul中注册服务时出错");
                }
            });

            // 应用停止时注销服务
            lifetime.ApplicationStopping.Register(async () =>
            {
                try
                {
                    await serviceDiscovery.DeregisterServiceAsync(serviceId);
                    logger?.LogInformation("服务已从Consul中注销: {ServiceId}", serviceId);
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex, "从Consul中注销服务时出错");
                }
            });

            // 添加健康检查中间件
            app.UseHealthChecks("/health");

            return app;
        }
    }
} 