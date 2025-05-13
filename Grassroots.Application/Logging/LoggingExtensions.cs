using Microsoft.Extensions.Logging;
using System;

namespace Grassroots.Application.Logging
{
    /// <summary>
    /// 日志扩展类
    /// </summary>
    public static class LoggingExtensions
    {
        /// <summary>
        /// 记录应用程序信息日志
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="message">日志消息</param>
        /// <param name="args">格式化参数</param>
        public static void LogAppInfo(this ILogger logger, string message, params object[] args)
        {
            logger.LogInformation(message, args);
        }

        /// <summary>
        /// 记录应用程序警告日志
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="message">日志消息</param>
        /// <param name="args">格式化参数</param>
        public static void LogAppWarning(this ILogger logger, string message, params object[] args)
        {
            logger.LogWarning(message, args);
        }

        /// <summary>
        /// 记录应用程序错误日志
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="message">日志消息</param>
        /// <param name="args">格式化参数</param>
        public static void LogAppError(this ILogger logger, string message, params object[] args)
        {
            logger.LogError(message, args);
        }

        /// <summary>
        /// 记录应用程序错误日志（带异常信息）
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="exception">异常</param>
        /// <param name="message">日志消息</param>
        /// <param name="args">格式化参数</param>
        public static void LogAppError(this ILogger logger, Exception exception, string message, params object[] args)
        {
            logger.LogError(exception, message, args);
        }

        /// <summary>
        /// 记录应用程序调试日志
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="message">日志消息</param>
        /// <param name="args">格式化参数</param>
        public static void LogAppDebug(this ILogger logger, string message, params object[] args)
        {
            logger.LogDebug(message, args);
        }

        /// <summary>
        /// 记录应用程序关键日志
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="message">日志消息</param>
        /// <param name="args">格式化参数</param>
        public static void LogAppCritical(this ILogger logger, string message, params object[] args)
        {
            logger.LogCritical(message, args);
        }

        /// <summary>
        /// 记录应用程序关键日志（带异常信息）
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="exception">异常</param>
        /// <param name="message">日志消息</param>
        /// <param name="args">格式化参数</param>
        public static void LogAppCritical(this ILogger logger, Exception exception, string message, params object[] args)
        {
            logger.LogCritical(exception, message, args);
        }
    }
} 