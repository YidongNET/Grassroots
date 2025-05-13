using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using Microsoft.Extensions.Logging;

namespace Grassroots.Infrastructure.ServiceDiscovery
{
    /// <summary>
    /// Consul服务发现实现
    /// </summary>
    public class ConsulServiceDiscovery : IServiceDiscovery
    {
        private readonly ILogger<ConsulServiceDiscovery> _logger;
        private readonly ConsulOptions _options;

        public ConsulServiceDiscovery(ILogger<ConsulServiceDiscovery> logger, ConsulOptions options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <inheritdoc />
        public async Task<bool> RegisterServiceAsync(string serviceId, string serviceName, string serviceAddress, int servicePort, string[] tags = null)
        {
            try
            {
                using var consulClient = new ConsulClient(config =>
                {
                    config.Address = new Uri(_options.Address);
                });

                var registration = new AgentServiceRegistration
                {
                    ID = serviceId,
                    Name = serviceName,
                    Address = serviceAddress,
                    Port = servicePort,
                    Tags = tags
                };

                // 添加健康检查
                if (_options.HealthCheck)
                {
                    var healthCheckPath = _options.HealthCheckPath ?? "/health";
                    var healthCheckInterval = _options.HealthCheckInterval;
                    var healthCheckTimeout = _options.HealthCheckTimeout;

                    var httpCheck = new AgentServiceCheck
                    {
                        DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
                        Interval = TimeSpan.FromSeconds(healthCheckInterval),
                        Timeout = TimeSpan.FromSeconds(healthCheckTimeout),
                        HTTP = $"http://{serviceAddress}:{servicePort}{healthCheckPath}"
                    };

                    registration.Check = httpCheck;
                }

                await consulClient.Agent.ServiceRegister(registration);
                _logger.LogInformation($"Service {serviceName} with id {serviceId} registered to Consul");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error registering service {serviceName} with id {serviceId} to Consul");
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<bool> DeregisterServiceAsync(string serviceId)
        {
            try
            {
                using var consulClient = new ConsulClient(config =>
                {
                    config.Address = new Uri(_options.Address);
                });

                await consulClient.Agent.ServiceDeregister(serviceId);
                _logger.LogInformation($"Service with id {serviceId} deregistered from Consul");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deregistering service with id {serviceId} from Consul");
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<IList<ServiceInstance>> GetServiceInstancesAsync(string serviceName)
        {
            try
            {
                using var consulClient = new ConsulClient(config =>
                {
                    config.Address = new Uri(_options.Address);
                });

                var services = await consulClient.Agent.Services();
                var serviceInstances = new List<ServiceInstance>();

                if (services.Response != null)
                {
                    var instances = services.Response.Where(s => s.Value.Service.Equals(serviceName, StringComparison.OrdinalIgnoreCase));
                    
                    foreach (var instance in instances)
                    {
                        var serviceInstance = new ServiceInstance
                        {
                            Id = instance.Value.ID,
                            Name = instance.Value.Service,
                            Address = instance.Value.Address,
                            Port = instance.Value.Port,
                            Tags = instance.Value.Tags,
                            Metadata = instance.Value.Meta,
                            IsHealthy = true // 默认为健康，后续可获取健康检查结果
                        };

                        serviceInstances.Add(serviceInstance);
                    }
                }

                return serviceInstances;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting service instances for {serviceName} from Consul");
                return new List<ServiceInstance>();
            }
        }

        /// <inheritdoc />
        public async Task<ServiceInstance> GetHealthyServiceInstanceAsync(string serviceName)
        {
            try
            {
                using var consulClient = new ConsulClient(config =>
                {
                    config.Address = new Uri(_options.Address);
                });

                var serviceEntries = await consulClient.Health.Service(serviceName, string.Empty, true);

                if (serviceEntries?.Response != null && serviceEntries.Response.Any())
                {
                    // 随机选择一个健康的服务实例
                    var random = new Random();
                    var index = random.Next(serviceEntries.Response.Length);
                    var entry = serviceEntries.Response[index];

                    var serviceInstance = new ServiceInstance
                    {
                        Id = entry.Service.ID,
                        Name = entry.Service.Service,
                        Address = entry.Service.Address,
                        Port = entry.Service.Port,
                        Tags = entry.Service.Tags,
                        Metadata = entry.Service.Meta,
                        IsHealthy = true
                    };

                    return serviceInstance;
                }
                
                _logger.LogWarning($"No healthy instances found for service {serviceName}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting healthy service instance for {serviceName} from Consul");
                return null;
            }
        }
    }
} 