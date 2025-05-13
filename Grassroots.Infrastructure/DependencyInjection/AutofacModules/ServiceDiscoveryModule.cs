using Autofac;
using Grassroots.Infrastructure.ServiceDiscovery;
using Microsoft.Extensions.Configuration;

namespace Grassroots.Infrastructure.DependencyInjection.AutofacModules
{
    public class ServiceDiscoveryModule : Autofac.Module
    {
        private readonly IConfiguration _configuration;

        public ServiceDiscoveryModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            // 注册Consul配置
            var consulOptions = new ConsulOptions();
            _configuration.GetSection("Consul").Bind(consulOptions);
            builder.RegisterInstance(consulOptions).AsSelf().SingleInstance();

            // 注册服务发现接口
            builder.RegisterType<ConsulServiceDiscovery>()
                .As<IServiceDiscovery>()
                .SingleInstance();

            // 注册HTTP客户端工厂
            builder.RegisterType<ServiceDiscoveryHttpClientFactory>()
                .AsSelf()
                .SingleInstance();
        }
    }
} 