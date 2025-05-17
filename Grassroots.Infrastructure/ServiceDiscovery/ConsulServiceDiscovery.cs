using Consul;
using Grassroots.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grassroots.Infrastructure.ServiceDiscovery
{
    /// <summary>
    /// Consul服务发现实现
    /// </summary>
    public class ConsulServiceDiscovery : IServiceDiscovery, IDisposable
    {
        private readonly ConsulOptions _options;
        private readonly ILogger<ConsulServiceDiscovery> _logger;
        private readonly ConsulClient _consulClient;
        private readonly Random _random = new Random();
        private bool _disposed = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options">Consul配置选项</param>
        /// <param name="logger">日志记录器</param>
        public ConsulServiceDiscovery(IOptions<ConsulOptions> options, ILogger<ConsulServiceDiscovery> logger)
        {
            _options = options.Value;
            _logger = logger;
            
            if (!_options.Enabled)
            {
                _logger.LogWarning("Consul服务发现未启用");
                return;
            }
            
            // 创建Consul客户端
            _consulClient = new ConsulClient(config =>
            {
                config.Address = new Uri(_options.ConsulAddress);
            });
            
            _logger.LogInformation("Consul服务发现已初始化, 地址: {ConsulAddress}", _options.ConsulAddress);
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <returns>注册结果</returns>
        public async Task<bool> RegisterServiceAsync()
        {
            if (!_options.Enabled || _consulClient == null)
            {
                _logger.LogWarning("Consul服务发现未启用，无法注册服务");
                return false;
            }
            
            try
            {
                // 创建健康检查
                var healthCheck = new AgentServiceCheck
                {
                    HTTP = $"http://{_options.ServiceAddress}:{_options.ServicePort}{_options.HealthCheck}",
                    Interval = TimeSpan.FromSeconds(_options.Interval),
                    Timeout = TimeSpan.FromSeconds(5),
                    DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1)
                };
                
                // 创建服务注册信息
                var registration = new AgentServiceRegistration
                {
                    ID = _options.ServiceId,
                    Name = _options.ServiceName,
                    Address = _options.ServiceAddress,
                    Port = _options.ServicePort,
                    Tags = _options.Tags.ToArray(),
                    Check = healthCheck
                };
                
                // 注册服务
                var result = await _consulClient.Agent.ServiceRegister(registration);
                
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    _logger.LogInformation("成功注册服务 {ServiceName} 到Consul", _options.ServiceName);
                    return true;
                }
                else
                {
                    _logger.LogError("注册服务 {ServiceName} 到Consul失败，状态码: {StatusCode}", _options.ServiceName, result.StatusCode);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "注册服务 {ServiceName} 到Consul时发生异常", _options.ServiceName);
                return false;
            }
        }

        /// <summary>
        /// 注销服务
        /// </summary>
        /// <returns>注销结果</returns>
        public async Task<bool> DeregisterServiceAsync()
        {
            if (!_options.Enabled || _consulClient == null)
            {
                _logger.LogWarning("Consul服务发现未启用，无法注销服务");
                return false;
            }
            
            try
            {
                var result = await _consulClient.Agent.ServiceDeregister(_options.ServiceId);
                
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    _logger.LogInformation("成功从Consul注销服务 {ServiceName}", _options.ServiceName);
                    return true;
                }
                else
                {
                    _logger.LogError("从Consul注销服务 {ServiceName} 失败，状态码: {StatusCode}", _options.ServiceName, result.StatusCode);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "从Consul注销服务 {ServiceName} 时发生异常", _options.ServiceName);
                return false;
            }
        }

        /// <summary>
        /// 发现服务
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns>服务地址列表</returns>
        public async Task<IList<string>> DiscoverServiceAsync(string serviceName)
        {
            if (!_options.Enabled || _consulClient == null)
            {
                _logger.LogWarning("Consul服务发现未启用，无法发现服务");
                return new List<string>();
            }
            
            try
            {
                // 获取健康的服务
                var queryResult = await _consulClient.Health.Service(serviceName, string.Empty, true);
                
                if (queryResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var services = queryResult.Response
                        .Select(service => $"http://{service.Service.Address}:{service.Service.Port}")
                        .ToList();
                    
                    _logger.LogInformation("从Consul发现服务 {ServiceName} 的 {Count} 个实例", serviceName, services.Count);
                    
                    return services;
                }
                else
                {
                    _logger.LogError("从Consul发现服务 {ServiceName} 失败，状态码: {StatusCode}", serviceName, queryResult.StatusCode);
                    return new List<string>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "从Consul发现服务 {ServiceName} 时发生异常", serviceName);
                return new List<string>();
            }
        }

        /// <summary>
        /// 获取健康服务
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns>健康的服务地址</returns>
        public async Task<string> GetHealthyServiceAsync(string serviceName)
        {
            if (!_options.Enabled || _consulClient == null)
            {
                _logger.LogWarning("Consul服务发现未启用，无法获取健康服务");
                return null;
            }
            
            var services = await DiscoverServiceAsync(serviceName);
            
            if (services.Count == 0)
            {
                _logger.LogWarning("未找到服务 {ServiceName} 的健康实例", serviceName);
                return null;
            }
            
            // 随机选择一个服务实例（简单的负载均衡）
            var service = services[_random.Next(services.Count)];
            _logger.LogInformation("选择服务 {ServiceName} 的实例: {ServiceUrl}", serviceName, service);
            
            return service;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">是否正在释放托管资源</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _consulClient?.Dispose();
            }

            _disposed = true;
        }
    }
} 