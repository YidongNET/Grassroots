using System.Collections.Generic;

namespace Grassroots.Infrastructure.ServiceDiscovery
{
    /// <summary>
    /// Consul配置选项
    /// </summary>
    public class ConsulOptions
    {
        /// <summary>
        /// 是否启用Consul
        /// </summary>
        public bool Enabled { get; set; } = true;
        
        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; set; } = "grassroots-api";
        
        /// <summary>
        /// 服务ID
        /// </summary>
        public string ServiceId { get; set; } = "grassroots-api-1";
        
        /// <summary>
        /// 服务地址
        /// </summary>
        public string ServiceAddress { get; set; } = "localhost";
        
        /// <summary>
        /// 服务端口
        /// </summary>
        public int ServicePort { get; set; } = 5000;
        
        /// <summary>
        /// Consul地址
        /// </summary>
        public string ConsulAddress { get; set; } = "http://localhost:8500";
        
        /// <summary>
        /// 健康检查地址
        /// </summary>
        public string HealthCheck { get; set; } = "/health";
        
        /// <summary>
        /// 服务标签
        /// </summary>
        public List<string> Tags { get; set; } = new List<string>();
        
        /// <summary>
        /// 健康检查间隔（秒）
        /// </summary>
        public int Interval { get; set; } = 10;
    }
} 