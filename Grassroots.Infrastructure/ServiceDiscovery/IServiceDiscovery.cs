using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Grassroots.Infrastructure.ServiceDiscovery
{
    /// <summary>
    /// 服务发现接口
    /// </summary>
    public interface IServiceDiscovery
    {
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="serviceId">服务ID</param>
        /// <param name="serviceName">服务名称</param>
        /// <param name="serviceAddress">服务地址</param>
        /// <param name="servicePort">服务端口</param>
        /// <param name="tags">标签</param>
        /// <returns>注册结果</returns>
        Task<bool> RegisterServiceAsync(string serviceId, string serviceName, string serviceAddress, int servicePort, string[] tags = null);

        /// <summary>
        /// 注销服务
        /// </summary>
        /// <param name="serviceId">服务ID</param>
        /// <returns>注销结果</returns>
        Task<bool> DeregisterServiceAsync(string serviceId);

        /// <summary>
        /// 获取服务实例
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns>服务实例列表</returns>
        Task<IList<ServiceInstance>> GetServiceInstancesAsync(string serviceName);

        /// <summary>
        /// 获取健康的服务实例
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns>健康的服务实例</returns>
        Task<ServiceInstance> GetHealthyServiceInstanceAsync(string serviceName);
    }

    /// <summary>
    /// 服务实例信息
    /// </summary>
    public class ServiceInstance
    {
        /// <summary>
        /// 服务ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 服务地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 服务端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 是否健康
        /// </summary>
        public bool IsHealthy { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public string[] Tags { get; set; }

        /// <summary>
        /// 服务元数据
        /// </summary>
        public IDictionary<string, string> Metadata { get; set; }
    }
} 