using Autofac;
using Grassroots.Application.Common.Interfaces;
using Grassroots.Domain.Repositories;
using Grassroots.Infrastructure.AutofacModules;
using Grassroots.Infrastructure.Persistence;
using Grassroots.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Grassroots.Infrastructure
{
    public static class DependencyInjection
    {
        // 保留原方法以兼容性，但不再推荐使用
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            return services;
        }

        // 新方法用于Autofac注册
        public static void RegisterInfrastructureServices(ContainerBuilder builder, IConfiguration configuration)
        {
            builder.RegisterModule(new InfrastructureModule(configuration));
        }
    }
} 