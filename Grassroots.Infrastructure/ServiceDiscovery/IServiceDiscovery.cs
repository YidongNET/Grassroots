using System.Threading.Tasks;

namespace Grassroots.Infrastructure.ServiceDiscovery;

/// <summary>
/// 服务发现接口
/// </summary>
public interface IServiceDiscovery
{
    /// <summary>
    /// 注册服务
    /// </summary>
    /// <returns>注册是否成功</returns>
    Task<bool> RegisterServiceAsync();
    
    /// <summary>
    /// 注销服务
    /// </summary>
    /// <returns>注销是否成功</returns>
    Task<bool> DeregisterServiceAsync();
    
    /// <summary>
    /// 发现服务
    /// </summary>
    /// <param name="serviceName">服务名称</param>
    /// <returns>服务地址列表</returns>
    Task<IList<string>> DiscoverServiceAsync(string serviceName);
    
    /// <summary>
    /// 获取指定服务的随机可用实例（简单负载均衡）
    /// </summary>
    /// <param name="serviceName">服务名称</param>
    /// <returns>服务地址</returns>
    Task<string> GetServiceInstanceAsync(string serviceName);
} 