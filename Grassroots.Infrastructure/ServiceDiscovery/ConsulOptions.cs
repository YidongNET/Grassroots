namespace Grassroots.Infrastructure.ServiceDiscovery;

/// <summary>
/// Consul服务配置选项
/// </summary>
public class ConsulOptions
{
    /// <summary>
    /// 是否启用服务注册
    /// </summary>
    public bool Enabled { get; set; } = true;
    
    /// <summary>
    /// Consul服务地址
    /// </summary>
    public string Address { get; set; } = "http://localhost:8500";
    
    /// <summary>
    /// 当前服务ID
    /// </summary>
    public string ServiceId { get; set; } = string.Empty;
    
    /// <summary>
    /// 当前服务名称
    /// </summary>
    public string ServiceName { get; set; } = "GrassrootsService";
    
    /// <summary>
    /// 当前服务IP地址
    /// </summary>
    public string ServiceAddress { get; set; } = "localhost";
    
    /// <summary>
    /// 当前服务端口
    /// </summary>
    public int ServicePort { get; set; } = 5000;
    
    /// <summary>
    /// 健康检查地址
    /// </summary>
    public string HealthCheck { get; set; } = "/health";
    
    /// <summary>
    /// 健康检查间隔（秒）
    /// </summary>
    public int HealthCheckInterval { get; set; } = 10;
    
    /// <summary>
    /// 健康检查超时（秒）
    /// </summary>
    public int HealthCheckTimeout { get; set; } = 5;
    
    /// <summary>
    /// 注销服务时是否从Consul移除服务
    /// </summary>
    public bool DeregisterCriticalServiceAfter { get; set; } = true;
    
    /// <summary>
    /// 服务注销超时（分钟）
    /// </summary>
    public int DeregisterCriticalServiceAfterMinutes { get; set; } = 1;
    
    /// <summary>
    /// 服务标签
    /// </summary>
    public string[] Tags { get; set; } = new[] { "api", "grassroots" };
} 