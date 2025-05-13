using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Grassroots.Infrastructure.ServiceDiscovery
{
    /// <summary>
    /// 服务发现HTTP客户端工厂
    /// </summary>
    public class ServiceDiscoveryHttpClientFactory : IDisposable
    {
        private readonly IServiceDiscovery _serviceDiscovery;
        private readonly ILogger<ServiceDiscoveryHttpClientFactory> _logger;
        private readonly HttpClient _httpClient;
        private bool _disposed;

        public ServiceDiscoveryHttpClientFactory(IServiceDiscovery serviceDiscovery, ILogger<ServiceDiscoveryHttpClientFactory> logger)
        {
            _serviceDiscovery = serviceDiscovery ?? throw new ArgumentNullException(nameof(serviceDiscovery));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// 创建HTTP客户端
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns>HTTP客户端</returns>
        public async Task<HttpClient> CreateClientAsync(string serviceName)
        {
            var serviceInstance = await _serviceDiscovery.GetHealthyServiceInstanceAsync(serviceName);
            if (serviceInstance == null)
            {
                throw new InvalidOperationException($"没有找到健康的服务实例: {serviceName}");
            }

            var baseAddress = $"http://{serviceInstance.Address}:{serviceInstance.Port}";
            _logger.LogInformation($"为服务 {serviceName} 创建HTTP客户端，基地址: {baseAddress}");
            
            _httpClient.BaseAddress = new Uri(baseAddress);
            _httpClient.DefaultRequestHeaders.Clear();
            
            return _httpClient;
        }

        /// <summary>
        /// 创建HTTP客户端并执行请求
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="serviceName">服务名称</param>
        /// <param name="requestFunc">请求函数</param>
        /// <returns>请求结果</returns>
        public async Task<T> ExecuteRequestAsync<T>(string serviceName, Func<HttpClient, Task<T>> requestFunc)
        {
            try
            {
                var client = await CreateClientAsync(serviceName);
                return await requestFunc(client);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"执行服务 {serviceName} 的请求时出错");
                throw;
            }
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
        /// <param name="disposing">是否释放托管资源</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _httpClient?.Dispose();
            }

            _disposed = true;
        }
    }
} 