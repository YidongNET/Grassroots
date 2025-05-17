using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace Grassroots.Api.Extensions
{
    /// <summary>
    /// 服务集合扩展
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加应用程序健康检查
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddAppHealthChecks(this IServiceCollection services)
        {
            services.AddHealthChecks();
            
            return services;
        }
        
        /// <summary>
        /// 使用健康检查
        /// </summary>
        /// <param name="app">应用程序构建器</param>
        /// <returns>应用程序构建器</returns>
        public static IApplicationBuilder UseAppHealthChecks(this IApplicationBuilder app)
        {
            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";
                    
                    var response = new
                    {
                        status = report.Status.ToString(),
                        checks = report.Entries.Select(e => new
                        {
                            name = e.Key,
                            status = e.Value.Status.ToString(),
                            description = e.Value.Description,
                            duration = e.Value.Duration
                        })
                    };
                    
                    await JsonSerializer.SerializeAsync(context.Response.Body, response);
                }
            });
            
            return app;
        }
    }
} 