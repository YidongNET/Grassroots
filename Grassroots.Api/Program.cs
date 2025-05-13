using Grassroots.Infrastructure.Data;
using Grassroots.Infrastructure.DependencyInjection;
using Grassroots.Infrastructure.Extensions;
using Grassroots.Infrastructure.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;
// 添加Autofac引用
using Autofac;
using Autofac.Extensions.DependencyInjection;
// 添加Serilog引用
using Serilog;

/*
 * Grassroots 应用启动配置
 * 
 * 本文件是应用的入口点，负责:
 * 1. 配置依赖注入容器（Autofac)
 * 2. 配置服务发现（Consul)
 * 3. 配置健康检查
 * 4. 注册各种中间件
 * 5. 初始化数据库
 * 6. 配置Serilog日志
 */

try
{
    var builder = WebApplication.CreateBuilder(args);

    // 配置Serilog
    // 检查日志是否启用
    bool serilogEnabled = LoggingConfigurationHelper.IsSerilogEnabled(builder.Configuration);
    
    // 创建日志配置并设置全局Logger
    Log.Logger = LoggingConfigurationHelper.CreateSerilogConfiguration(builder.Configuration).CreateLogger();
    
    // 只有在启用的情况下才注册Serilog服务
    if (serilogEnabled)
    {
        builder.Host.UseSerilog();
    }
    else
    {
        Console.WriteLine("Serilog 日志功能已禁用，通过修改配置文件中的 Serilog:Enabled 设置为 true 可以重新启用。");
    }

    // 配置使用Autofac作为DI容器，替换默认的.NET DI容器
    // Autofac提供更强大的依赖注入功能，包括属性注入、模块化注册和动态代理
    builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
    builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
    {
        // 注册所有Grassroots模块 - 这将注册基础设施、应用层、数据库和服务发现模块
        containerBuilder.RegisterGrassrootsModules(builder.Configuration);
    });

    // 添加控制器和API浏览器
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    // 配置Swagger，添加XML注释
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Grassroots API",
            Version = "v1",
            Description = "基于领域驱动设计的现代化应用框架API",
            Contact = new OpenApiContact
            {
                Name = "Grassroots Team",
                Email = "admin@yidongnet.com",
                Url = new Uri("https://github.com/yidongnet/Grassroots")
            }
        });

        // 启用XML注释，用于Swagger文档
        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);

        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }
    });

    // 添加服务发现 - 基于配置决定是否启用Consul服务发现
    // 可通过appsettings.json中的Features:ServiceDiscovery:Enabled配置项控制
    builder.Services.AddServiceDiscovery(builder.Configuration);

    // 添加健康检查 - 用于服务监控和自动故障恢复
    builder.Services.AddHealthChecks();

    var app = builder.Build();

    // 初始化数据库 - 创建数据库并应用迁移（如果需要）
    await DbInitializer.InitializeAsync(app.Services);

    // 配置HTTP请求管道
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Grassroots API v1");
            c.RoutePrefix = "swagger";
        });
    }

    // 仅在日志启用的情况下使用Serilog请求日志中间件
    if (serilogEnabled)
    {
        app.UseSerilogRequestLogging();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    // 使用服务注册（包含健康检查终结点）
    // 将服务注册到Consul，并在应用程序关闭时自动注销
    // 仅当Features:ServiceDiscovery:Enabled和Consul:Enabled都为true时才会注册
    app.UseServiceRegistration(app.Lifetime, builder.Configuration);

    if (serilogEnabled)
    {
        Log.Information("应用程序启动成功");
    }
    else
    {
        Console.WriteLine("应用程序启动成功");
    }
    
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "应用程序启动失败");
}
finally
{
    Log.CloseAndFlush();
}
