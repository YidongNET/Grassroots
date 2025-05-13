namespace Grassroots.Infrastructure.ServiceDiscovery
{
    /// <summary>
    /// Consul选项配置
    /// </summary>
    public class ConsulOptions
    {
        /// <summary>
        /// 是否启用Consul服务
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Consul服务地址
        /// </summary>
        public string Address { get; set; } = "http://localhost:8500";

        /// <summary>
        /// 是否启用健康检查
        /// </summary>
        public bool HealthCheck { get; set; } = true;

        /// <summary>
        /// 健康检查路径
        /// </summary>
        public string HealthCheckPath { get; set; } = "/health";

        /// <summary>
        /// 健康检查间隔（秒）
        /// </summary>
        public int HealthCheckInterval { get; set; } = 10;

        /// <summary>
        /// 健康检查超时（秒）
        /// </summary>
        public int HealthCheckTimeout { get; set; } = 5;
    }
} 