using Grassroots.Application.Logging;
using Grassroots.Infrastructure.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text.Json;

namespace Grassroots.Api.Controllers
{
    /// <summary>
    /// 日志示例控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class LoggingDemoController : ControllerBase
    {
        private readonly ILogger<LoggingDemoController> _logger;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="configuration">配置</param>
        public LoggingDemoController(ILogger<LoggingDemoController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// 记录信息日志示例
        /// </summary>
        /// <returns>操作结果</returns>
        [HttpGet("info")]
        public IActionResult LogInfo()
        {
            _logger.LogAppInfo("这是一条信息日志，请求时间: {Time}", DateTime.Now);
            return Ok("信息日志已记录");
        }

        /// <summary>
        /// 记录警告日志示例
        /// </summary>
        /// <returns>操作结果</returns>
        [HttpGet("warning")]
        public IActionResult LogWarning()
        {
            _logger.LogAppWarning("这是一条警告日志，请求时间: {Time}", DateTime.Now);
            return Ok("警告日志已记录");
        }

        /// <summary>
        /// 记录错误日志示例
        /// </summary>
        /// <returns>操作结果</returns>
        [HttpGet("error")]
        public IActionResult LogError()
        {
            _logger.LogAppError("这是一条错误日志，请求时间: {Time}", DateTime.Now);
            return Ok("错误日志已记录");
        }

        /// <summary>
        /// 记录异常日志示例
        /// </summary>
        /// <returns>操作结果</returns>
        [HttpGet("exception")]
        public IActionResult LogException()
        {
            try
            {
                // 模拟异常
                throw new InvalidOperationException("这是一个模拟的异常");
            }
            catch (Exception ex)
            {
                // 使用扩展方法记录带异常的错误日志
                _logger.LogAppError(ex, "处理请求时发生异常，请求时间: {Time}", DateTime.Now);
                return StatusCode(500, "异常日志已记录");
            }
        }

        /// <summary>
        /// 记录调试日志示例
        /// </summary>
        /// <returns>操作结果</returns>
        [HttpGet("debug")]
        public IActionResult LogDebug()
        {
            _logger.LogAppDebug("这是一条调试日志，请求时间: {Time}", DateTime.Now);
            return Ok("调试日志已记录");
        }

        /// <summary>
        /// 记录关键日志示例
        /// </summary>
        /// <returns>操作结果</returns>
        [HttpGet("critical")]
        public IActionResult LogCritical()
        {
            _logger.LogAppCritical("这是一条关键日志，请求时间: {Time}", DateTime.Now);
            return Ok("关键日志已记录");
        }

        /// <summary>
        /// 测试所有日志级别
        /// </summary>
        /// <returns>操作结果</returns>
        [HttpGet("all-levels")]
        public IActionResult LogAllLevels()
        {
            // 使用标准ILogger接口方法
            _logger.LogTrace("这是Trace级别日志");
            _logger.LogDebug("这是Debug级别日志");
            _logger.LogInformation("这是Information级别日志");
            _logger.LogWarning("这是Warning级别日志");
            _logger.LogError("这是Error级别日志");
            _logger.LogCritical("这是Critical级别日志");

            // 使用扩展方法
            _logger.LogAppDebug("这是通过扩展方法的Debug级别日志");
            _logger.LogAppInfo("这是通过扩展方法的Info级别日志");
            _logger.LogAppWarning("这是通过扩展方法的Warning级别日志");
            _logger.LogAppError("这是通过扩展方法的Error级别日志");
            _logger.LogAppCritical("这是通过扩展方法的Critical级别日志");

            // 使用结构化日志（带参数）
            _logger.LogInformation("带结构化参数的日志: {Parameter1}, {Parameter2}", "值1", 42);

            return Ok("所有级别的日志已记录");
        }

        /// <summary>
        /// 获取日志开关状态
        /// </summary>
        /// <returns>日志开关状态</returns>
        [HttpGet("status")]
        public IActionResult GetLogStatus()
        {
            bool isEnabled = LoggingConfigurationHelper.IsSerilogEnabled(_configuration);
            return Ok(new { Enabled = isEnabled });
        }

        /// <summary>
        /// 切换日志开关状态
        /// </summary>
        /// <param name="enabled">是否启用日志</param>
        /// <returns>操作结果</returns>
        [HttpPost("toggle")]
        public IActionResult ToggleLogStatus([FromQuery] bool enabled)
        {
            try
            {
                // 获取appsettings.json文件路径
                string configPath = "appsettings.json";
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    // 如果是开发环境，则修改appsettings.Development.json
                    configPath = "appsettings.Development.json";
                }

                // 读取配置文件
                string json = System.IO.File.ReadAllText(configPath);
                using JsonDocument doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // 创建一个可修改的副本
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                JsonElement serilogElement;
                var jsonObj = JsonSerializer.Deserialize<System.Text.Json.Nodes.JsonObject>(json, options);
                
                if (jsonObj["Serilog"] != null)
                {
                    jsonObj["Serilog"]["Enabled"] = enabled;
                }

                // 写回文件
                string updatedJson = JsonSerializer.Serialize(jsonObj, options);
                System.IO.File.WriteAllText(configPath, updatedJson);

                return Ok(new { Success = true, Message = $"日志功能已{(enabled ? "启用" : "禁用")}，请重启应用以应用更改" });
            }
            catch (Exception ex)
            {
                _logger.LogAppError(ex, "切换日志状态时发生错误");
                return StatusCode(500, new { Success = false, Message = "修改配置文件失败: " + ex.Message });
            }
        }
    }
} 