using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading.Tasks;

namespace Grassroots.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly HealthCheckService _healthCheckService;

        public HealthController(HealthCheckService healthCheckService)
        {
            _healthCheckService = healthCheckService;
        }

        /// <summary>
        /// 健康检查接口
        /// </summary>
        /// <returns>健康状态</returns>
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get()
        {
            var report = await _healthCheckService.CheckHealthAsync();
            return report.Status == HealthStatus.Healthy ? 
                Ok(report) : 
                StatusCode(503, report);
        }
    }
} 