using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Consul;
using Microsoft.Extensions.Options;
using Serilog;

namespace Grassroots.Infrastructure.ServiceDiscovery;

/// <summary>
/// Consul服务发现实现
/// </summary>
public class ConsulServiceDiscovery : IServiceDiscovery
{
    private readonly ConsulOptions _options;
    private readonly IConsulClient _consulClient;
    private readonly ILogger _logger;
    private readonly Random _random = new Random();

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="options">Consul配置选项</param>
    /// <param name="logger">日志</param>
    public ConsulServiceDiscovery(IOptions<ConsulOptions> options, ILogger logger)
    {
        _options = options.Value;
        _logger = logger;
        
        // 创建Consul客户端
        _consulClient = new ConsulClient(config =>
        {
            config.Address = new Uri(_options.Address);
        });
    }

    /// <summary>
    /// 注册服务
    /// </summary>
    /// <returns>注册是否成功</returns>
    public async Task<bool> RegisterServiceAsync()
    {
        try
        {
            if (!_options.Enabled)
            {
                _logger.Information("Consul服务注册已禁用");
                return false;
            }
            
            var serviceId = string.IsNullOrEmpty(_options.ServiceId)
                ? $"{_options.ServiceName}-{_options.ServiceAddress}-{_options.ServicePort}"
                : _options.ServiceId;
            
            // 创建健康检查
            var checkAddress = $"http://{_options.ServiceAddress}:{_options.ServicePort}{_options.HealthCheck}";
            
            var registration = new AgentServiceRegistration
            {
                ID = serviceId,
                Name = _options.ServiceName,
                Address = _options.ServiceAddress,
                Port = _options.ServicePort,
                Tags = _options.Tags,
                Check = new AgentServiceCheck
                {
                    HTTP = checkAddress,
                    Interval = TimeSpan.FromSeconds(_options.HealthCheckInterval),
                    Timeout = TimeSpan.FromSeconds(_options.HealthCheckTimeout),
                    DeregisterCriticalServiceAfter = _options.DeregisterCriticalServiceAfter 
                        ? TimeSpan.FromMinutes(_options.DeregisterCriticalServiceAfterMinutes) 
                        : TimeSpan.Zero
                }
            };
            
            // 注册服务
            var result = await _consulClient.Agent.ServiceRegister(registration);
            
            if (result.StatusCode == HttpStatusCode.OK)
            {
                _logger.Information("成功注册服务到Consul: {ServiceId}", serviceId);
                return true;
            }
            
            _logger.Warning("注册服务到Consul失败: {StatusCode}", result.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "注册服务到Consul时发生异常");
            return false;
        }
    }

    /// <summary>
    /// 注销服务
    /// </summary>
    /// <returns>注销是否成功</returns>
    public async Task<bool> DeregisterServiceAsync()
    {
        try
        {
            if (!_options.Enabled)
            {
                return false;
            }
            
            var serviceId = string.IsNullOrEmpty(_options.ServiceId)
                ? $"{_options.ServiceName}-{_options.ServiceAddress}-{_options.ServicePort}"
                : _options.ServiceId;
            
            var result = await _consulClient.Agent.ServiceDeregister(serviceId);
            
            if (result.StatusCode == HttpStatusCode.OK)
            {
                _logger.Information("成功从Consul注销服务: {ServiceId}", serviceId);
                return true;
            }
            
            _logger.Warning("从Consul注销服务失败: {StatusCode}", result.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "从Consul注销服务时发生异常");
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
        try
        {
            var queryResult = await _consulClient.Health.Service(serviceName, string.Empty, true);
            
            if (queryResult.StatusCode != HttpStatusCode.OK)
            {
                _logger.Warning("从Consul查询服务失败: {StatusCode}", queryResult.StatusCode);
                return new List<string>();
            }
            
            var services = queryResult.Response;
            var serviceUrls = services
                .Select(service => $"http://{service.Service.Address}:{service.Service.Port}")
                .ToList();
            
            _logger.Debug("从Consul发现服务: {ServiceName}, 实例数: {Count}", 
                serviceName, serviceUrls.Count);
            
            return serviceUrls;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "从Consul发现服务时发生异常: {ServiceName}", serviceName);
            return new List<string>();
        }
    }

    /// <summary>
    /// 获取指定服务的随机可用实例（简单负载均衡）
    /// </summary>
    /// <param name="serviceName">服务名称</param>
    /// <returns>服务地址</returns>
    public async Task<string> GetServiceInstanceAsync(string serviceName)
    {
        var services = await DiscoverServiceAsync(serviceName);
        
        if (services.Count == 0)
        {
            _logger.Warning("没有可用的服务实例: {ServiceName}", serviceName);
            return string.Empty;
        }
        
        // 随机选择一个服务实例
        var index = _random.Next(services.Count);
        var serviceUrl = services[index];
        
        _logger.Debug("为{ServiceName}选择服务实例: {ServiceUrl}", serviceName, serviceUrl);
        
        return serviceUrl;
    }
} 