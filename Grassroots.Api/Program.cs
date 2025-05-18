using Autofac;
using Autofac.Extensions.DependencyInjection;
using Grassroots.Api.Converters;
using Grassroots.Api.Extensions;
using Grassroots.Application;
using Grassroots.Application.Common.Interfaces;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;

try
{
    // 创建初始日志记录器配置 - 仅控制台输出，用于应用启动前的日志
    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateBootstrapLogger();
    
    Log.Information("启动Grassroots服务");
    
    var builder = WebApplication.CreateBuilder(args);
    
    // 获取Serilog启用状态
    var serilogEnabled = builder.Configuration.GetValue<bool>("Serilog:Enabled", true);
    
    if (serilogEnabled)
    {
        Log.Information("Serilog已启用，将使用完整的结构化日志记录");
        
        // 配置Serilog
        // 使用基于配置的结构化日志，支持文件、控制台、数据库等多种输出
        builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext());
    }
    else
    {
        Log.Information("Serilog已禁用，将仅使用控制台基本日志");
        
        // 使用基本日志（仅控制台输出）
        builder.Host.UseSerilog(new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .CreateLogger());
    }

    // 使用Autofac作为IoC容器
    // Autofac提供更灵活的依赖注入能力，支持模块化配置和高级注册特性
    builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

    // 注册控制器和其它ASP.NET Core服务
    // 配置JSON序列化，将long类型转为string以避免JavaScript中的数值精度问题
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            // 配置System.Text.Json将长整型数字序列化为字符串
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.Converters.Add(new LongToStringConverter());
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });
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
                Name = "YidongNET",
                Email = "admin@yidongnet.com",
                Url = new Uri("https://github.com/YidongNET/Grassroots")
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



    // 添加健康检查
    // 用于监控和报告应用程序的健康状态
    builder.Services.AddAppHealthChecks();

    // 配置Autofac容器
    builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
    {
        // 注册Application服务
        // 应用层的服务直接注册
        Grassroots.Application.DependencyInjection.RegisterApplicationServices(containerBuilder);

        try
        {
            // 通过反射注册Infrastructure服务（依赖倒置）
            // 这是依赖倒置原则(DIP)的关键实现：
            // 1. API层不直接依赖于Infrastructure层
            // 2. 通过反射动态加载Infrastructure层的注册方法
            // 3. 这使得高层模块不依赖于低层模块的具体实现
            var infrastructureAssembly = Assembly.Load("Grassroots.Infrastructure");
            var infrastructureDIType = infrastructureAssembly.GetType("Grassroots.Infrastructure.DependencyInjection");
            var registerMethod = infrastructureDIType?.GetMethod("RegisterInfrastructureServices");
            registerMethod?.Invoke(null, new object[] { containerBuilder, builder.Configuration });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "无法加载Infrastructure层");
            
            // 使用备用方法注册基本服务
            // 当Infrastructure层无法加载时，提供基本功能以保证应用可以启动
            builder.Services.AddDbContext<DbContext>(options => 
                options.UseInMemoryDatabase("FallbackDb"));
        }
    });

    var app = builder.Build();

    // 配置HTTP请求管道
    // 请求处理中间件按照顺序执行
    if (app.Environment.IsDevelopment())
    {
        // 开发环境启用Swagger文档
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // 启用HTTPS重定向
    app.UseHttpsRedirection();
    
    // 启用授权中间件
    app.UseAuthorization();
    
    // 映射控制器路由
    app.MapControllers();

    // 使用健康检查中间件
    // 提供/health端点用于监控系统
    app.UseAppHealthChecks();

    // 根据配置决定是否使用Consul服务注册
    // Consul用于服务发现和服务网格
    var consulEnabled = app.Configuration.GetValue<bool>("Consul:Enabled", false);
    if (consulEnabled)
    {
        app.Logger.LogInformation("Consul服务注册已启用");
        app.UseConsul();
    }
    else
    {
        app.Logger.LogInformation("Consul服务注册已禁用");
    }

    // 使用Serilog请求日志中间件（仅当Serilog完全启用时）
    // 记录HTTP请求的详细信息，包括路径、方法、状态码和响应时间
    if (serilogEnabled)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        });
    }

    // 启动应用程序
    app.Run();
}
catch (Exception ex)
{
    // 捕获并记录启动过程中的任何异常
    Log.Fatal(ex, "应用程序启动失败");
}
finally
{
    // 确保日志被刷新并关闭
    Log.CloseAndFlush();
}
