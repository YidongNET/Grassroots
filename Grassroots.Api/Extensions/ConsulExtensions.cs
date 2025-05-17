using Grassroots.Application.Common.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Grassroots.Api.Extensions
{
    /// <summary>
    /// Consul扩展
    /// </summary>
    public static class ConsulExtensions
    {
        /// <summary>
        /// 使用Consul服务注册
        /// </summary>
        /// <param name="app">应用程序构建器</param>
        /// <returns>应用程序构建器</returns>
        public static IApplicationBuilder UseConsul(this IApplicationBuilder app)
        {
            try
            {
                var logger = app.ApplicationServices.GetRequiredService<ILogger<IApplicationBuilder>>();
                var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
                
                // 尝试获取服务发现服务
                if (!app.ApplicationServices.GetService<IServiceDiscovery>()?.RegisterServiceAsync().GetAwaiter().GetResult() ?? false)
                {
                    logger.LogWarning("没有找到IServiceDiscovery服务或服务注册失败，Consul服务注册将不可用");
                    return app;
                }
                
                var serviceDiscovery = app.ApplicationServices.GetRequiredService<IServiceDiscovery>();
                
                // 应用程序启动时注册服务
                lifetime.ApplicationStarted.Register(async () =>
                {
                    try
                    {
                        logger.LogInformation("应用程序已启动，正在注册到Consul...");
                        var result = await serviceDiscovery.RegisterServiceAsync();
                        
                        if (result)
                        {
                            logger.LogInformation("服务已成功注册到Consul");
                        }
                        else
                        {
                            logger.LogWarning("服务注册到Consul失败");
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "注册到Consul时发生异常");
                    }
                });
                
                // 应用程序停止时注销服务
                lifetime.ApplicationStopping.Register(async () =>
                {
                    try
                    {
                        logger.LogInformation("应用程序正在停止，正在从Consul注销服务...");
                        var result = await serviceDiscovery.DeregisterServiceAsync();
                        
                        if (result)
                        {
                            logger.LogInformation("服务已成功从Consul注销");
                        }
                        else
                        {
                            logger.LogWarning("服务从Consul注销失败");
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "从Consul注销时发生异常");
                    }
                });
            }
            catch (Exception ex)
            {
                var logger = app.ApplicationServices.GetService<ILogger<IApplicationBuilder>>();
                logger?.LogError(ex, "配置Consul服务注册时发生异常");
            }
            
            return app;
        }
    }
} 