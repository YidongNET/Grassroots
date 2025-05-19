using Microsoft.AspNetCore.Mvc;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Grassroots.Api.Controllers;

/// <summary>
/// 健康检查控制器
/// </summary>
[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    private readonly ILogger _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志记录器</param>
    public HealthController(ILogger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 获取服务健康状态
    /// </summary>
    /// <returns>健康状态</returns>
    [HttpGet]
    public IActionResult Get()
    {
        _logger.Debug("健康检查请求");
        return Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow });
    }
} 