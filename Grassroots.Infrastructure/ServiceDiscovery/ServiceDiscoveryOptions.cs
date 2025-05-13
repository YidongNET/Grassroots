namespace Grassroots.Infrastructure.ServiceDiscovery
{
    /// <summary>
    /// 服务发现选项
    /// </summary>
    public class ServiceDiscoveryOptions
    {
        /// <summary>
        /// 是否启用服务发现
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// 服务发现提供程序
        /// </summary>
        public string Provider { get; set; } = "Consul";
    }

    /// <summary>
    /// 特性选项
    /// </summary>
    public class FeaturesOptions
    {
        /// <summary>
        /// 服务发现特性
        /// </summary>
        public ServiceDiscoveryOptions ServiceDiscovery { get; set; } = new ServiceDiscoveryOptions();
    }
} 