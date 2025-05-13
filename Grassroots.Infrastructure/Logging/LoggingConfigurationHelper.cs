using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;

namespace Grassroots.Infrastructure.Logging
{
    /// <summary>
    /// 日志配置帮助类
    /// </summary>
    public static class LoggingConfigurationHelper
    {
        /// <summary>
        /// 创建Serilog日志配置
        /// </summary>
        /// <param name="configuration">应用配置</param>
        /// <returns>日志配置</returns>
        public static LoggerConfiguration CreateSerilogConfiguration(IConfiguration configuration)
        {
            // 检查Serilog是否启用
            bool isEnabled = true;
            if (configuration.GetSection("Serilog:Enabled").Exists())
            {
                isEnabled = configuration.GetValue<bool>("Serilog:Enabled");
            }

            // 创建基本配置
            var loggerConfig = new LoggerConfiguration();
            
            if (!isEnabled)
            {
                // 如果日志被禁用，则返回一个最小日志级别设置为Fatal的配置
                // 这样只有Fatal级别的日志才会被记录，实际上等同于禁用了大部分日志
                return loggerConfig.MinimumLevel.Fatal();
            }

            // 正常配置日志
            loggerConfig = loggerConfig
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId();

            // 如果配置文件中没有设置最小日志级别，则使用默认值
            if (!configuration.GetSection("Serilog:MinimumLevel:Default").Exists())
            {
                loggerConfig.MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("System", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning);
            }

            // 如果配置文件中没有设置输出目标，则使用控制台和文件作为默认输出
            if (!configuration.GetSection("Serilog:WriteTo").Exists())
            {
                loggerConfig
                    .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                    .WriteTo.File(
                        path: "logs/log-.txt",
                        rollingInterval: RollingInterval.Day,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                        fileSizeLimitBytes: 10 * 1024 * 1024,
                        retainedFileCountLimit: 30);
            }

            return loggerConfig;
        }

        /// <summary>
        /// 检查Serilog是否启用
        /// </summary>
        /// <param name="configuration">应用配置</param>
        /// <returns>是否启用</returns>
        public static bool IsSerilogEnabled(IConfiguration configuration)
        {
            if (configuration.GetSection("Serilog:Enabled").Exists())
            {
                return configuration.GetValue<bool>("Serilog:Enabled");
            }
            return true; // 默认启用
        }
    }
} 