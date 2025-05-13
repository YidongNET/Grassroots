using Grassroots.Infrastructure.ServiceDiscovery;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Grassroots.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceDiscoveryDemoController : ControllerBase
    {
        private readonly IServiceDiscovery _serviceDiscovery;
        private readonly ServiceDiscoveryHttpClientFactory _httpClientFactory;

        public ServiceDiscoveryDemoController(
            IServiceDiscovery serviceDiscovery, 
            ServiceDiscoveryHttpClientFactory httpClientFactory)
        {
            _serviceDiscovery = serviceDiscovery ?? throw new ArgumentNullException(nameof(serviceDiscovery));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        /// <summary>
        /// 获取所有服务实例
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns>服务实例列表</returns>
        [HttpGet("services/{serviceName}")]
        public async Task<IActionResult> GetServices(string serviceName)
        {
            var services = await _serviceDiscovery.GetServiceInstancesAsync(serviceName);
            return Ok(services);
        }

        /// <summary>
        /// 获取健康的服务实例
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns>健康的服务实例</returns>
        [HttpGet("services/{serviceName}/healthy")]
        public async Task<IActionResult> GetHealthyService(string serviceName)
        {
            var service = await _serviceDiscovery.GetHealthyServiceInstanceAsync(serviceName);
            if (service == null)
            {
                return NotFound($"找不到健康的 {serviceName} 服务实例");
            }
            
            return Ok(service);
        }

        /// <summary>
        /// 调用示例服务
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns>服务响应</returns>
        [HttpGet("call/{serviceName}")]
        public async Task<IActionResult> CallService(string serviceName, string path = "/api/values")
        {
            try
            {
                var result = await _httpClientFactory.ExecuteRequestAsync<JsonDocument>(serviceName, async (client) =>
                {
                    var response = await client.GetAsync(path);
                    response.EnsureSuccessStatusCode();
                    
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonDocument.Parse(content);
                });
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"调用服务 {serviceName} 失败: {ex.Message}");
            }
        }
    }
} 