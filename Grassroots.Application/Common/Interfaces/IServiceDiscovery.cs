using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Grassroots.Application.Common.Interfaces
{
    /// <summary>
    /// 服务发现接口
    /// </summary>
    public interface IServiceDiscovery
    {
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <returns>注册结果</returns>
        Task<bool> RegisterServiceAsync();
        
        /// <summary>
        /// 注销服务
        /// </summary>
        /// <returns>注销结果</returns>
        Task<bool> DeregisterServiceAsync();
        
        /// <summary>
        /// 发现服务
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns>服务地址列表</returns>
        Task<IList<string>> DiscoverServiceAsync(string serviceName);
        
        /// <summary>
        /// 获取健康服务
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns>健康的服务地址</returns>
        Task<string> GetHealthyServiceAsync(string serviceName);
    }
} 