using Autofac;
using Autofac.Extensions.DependencyInjection;
using Grassroots.Application;
using Grassroots.Application.DependencyInjection;
using Grassroots.Infrastructure;
using Grassroots.Infrastructure.Data;
using Grassroots.Infrastructure.DependencyInjection;
using Grassroots.Infrastructure.ServiceDiscovery;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System.Text.Json;
using System.Text.Json.Serialization;
using Grassroots.Api.Middleware;

// 创建Web应用构建器
var builder = WebApplication.CreateBuilder(args);

// 检查Serilog是否启用
// 通过配置决定是否使用结构化日志记录
bool enableSerilog = true;
if (builder.Configuration.GetSection("Serilog")["Enabled"] is string enabledStr)
{
    bool.TryParse(enabledStr, out enableSerilog);
}

if (enableSerilog)
{
    // 配置Serilog
    // 创建启动引导日志器，用于记录应用程序启动过程中的日志
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithThreadId()
        .WriteTo.Console()
        .CreateBootstrapLogger(); // 创建一个预启动的日志器
}
else
{
    // 配置一个空日志器
    // 当禁用日志时，确保代码中的日志调用不会引发错误
    Log.Logger = new LoggerConfiguration().CreateLogger();
}

try
{
    if (enableSerilog)
    {
        Log.Information("启动 Grassroots API 应用程序");
    }

    // 将Serilog配置为使用appsettings.json中的设置
    // 这样可以在应用运行期间动态调整日志级别和输出目标
    if (enableSerilog)
    {
        builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithThreadId());
    }

    // 配置使用Autofac作为服务提供者工厂
    // Autofac提供了更强大的依赖注入功能，如属性注入、模块化注册等
    builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

    // 配置Autofac容器
    // 注册应用层和基础设施层的服务模块
    builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder => 
    {
        // 注册应用层模块
        // 包含应用服务、命令/查询处理器、验证器等
        containerBuilder.RegisterModule(new ApplicationModule());
        
        // 注册基础设施层模块
        // 包含仓储、工作单元、事件服务、外部集成等
        containerBuilder.RegisterModule(new InfrastructureModule(builder.Configuration));
        
        // 如果Serilog未启用，注册一个空的ILogger
        if (!enableSerilog)
        {
            containerBuilder.RegisterInstance(Log.Logger).As<Serilog.ILogger>().SingleInstance();
        }
    });

    // 添加控制器并配置JSON序列化选项
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            // 配置长整型转字符串
            // 避免JavaScript客户端处理大整数时精度丢失问题
            options.JsonSerializerOptions.Converters.Add(new LongToStringConverter());
            options.JsonSerializerOptions.Converters.Add(new NullableLongToStringConverter());
            
            // 设置其他JSON序列化选项
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });
        
    // 添加API端点探索服务
    // 支持Swagger和OpenAPI客户端自动发现API端点
    builder.Services.AddEndpointsApiExplorer();

    // 配置Swagger，添加XML注释
    // 为API生成交互式文档，便于开发和测试
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
    });

    // 添加基础设施层服务
    // 注意：这里仅配置DbContext，其他服务已通过Autofac模块注册
    var databaseSection = builder.Configuration.GetSection("Database");
    var databaseTypeStr = databaseSection["ProviderType"] ?? "SqlServer";
    if (!Enum.TryParse<DatabaseType>(databaseTypeStr, true, out var databaseType))
    {
        databaseType = DatabaseType.SqlServer; // 默认使用SQL Server
    }
    string connectionStringName = "DefaultConnection";
    
    // 使用工厂模式配置DbContext，支持多数据库切换
    Grassroots.Infrastructure.Data.DbContextFactory.ConfigureDbContext(
        builder.Services, 
        builder.Configuration, 
        databaseType, 
        connectionStringName);
    
    // 配置Consul选项
    // 用于服务注册与发现，支持微服务架构
    builder.Services.Configure<ConsulOptions>(builder.Configuration.GetSection("Consul"));

    // 构建应用
    var app = builder.Build();

    // 配置HTTP请求管道
    // 请求处理中间件按照顺序执行
    if (app.Environment.IsDevelopment())
    {
        // 开发环境启用Swagger文档
        // 方便开发人员测试和调试API
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Grassroots API v1");
            c.RoutePrefix = "swagger";
        });
    }

    // 添加Serilog请求日志记录中间件
    // 记录所有HTTP请求的处理情况，包括路径、状态码和处理时间
    if (enableSerilog)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";
        });
    }

    // 使用HTTPS重定向
    // 确保所有请求都通过安全连接
    app.UseHttpsRedirection();

    // 使用身份验证和授权
    // 控制API访问权限
    app.UseAuthorization();

    // 添加全局异常处理中间件
    app.UseGlobalExceptionHandler();

    // 映射控制器路由
    // 将请求路由到相应的控制器
    app.MapControllers();
    
    // 注册Consul服务
    // 将服务实例注册到服务发现系统，支持微服务架构
    var consulOptions = app.Services.GetRequiredService<IOptions<ConsulOptions>>().Value;
    if (consulOptions.Enabled)
    {
        // 获取服务发现服务
        var serviceDiscovery = app.Services.GetRequiredService<IServiceDiscovery>();
        
        // 应用启动时注册服务
        if (enableSerilog)
        {
            Log.Information("正在注册服务到Consul: {ServiceName}", consulOptions.ServiceName);
        }
        
        // 异步等待服务注册完成
        await serviceDiscovery.RegisterServiceAsync();
        
        // 应用程序终止时注销服务
        // 确保服务被正常移除，避免服务发现错误
        app.Lifetime.ApplicationStopping.Register(async () =>
        {
            if (enableSerilog)
            {
                Log.Information("正在从Consul注销服务: {ServiceName}", consulOptions.ServiceName);
            }
            await serviceDiscovery.DeregisterServiceAsync();
        });
    }

    // 运行应用程序
    app.Run();
}
catch (Exception ex)
{
    // 记录启动失败异常
    if (enableSerilog)
    {
        Log.Fatal(ex, "应用程序启动失败");
    }
}
finally
{
    // 确保在应用退出时关闭并刷新日志
    if (enableSerilog)
    {
        Log.CloseAndFlush();
    }
}

/// <summary>
/// 将long类型转换为string的JSON转换器
/// </summary>
public class LongToStringConverter : JsonConverter<long>
{
    public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            return long.Parse(reader.GetString()!);
        }
        return reader.GetInt64();
    }

    public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}

/// <summary>
/// 将可空long类型转换为string的JSON转换器
/// </summary>
public class NullableLongToStringConverter : JsonConverter<long?>
{
    public override long? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }
        
        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString();
            if (string.IsNullOrEmpty(stringValue))
            {
                return null;
            }
            return long.Parse(stringValue);
        }
        
        return reader.GetInt64();
    }

    public override void Write(Utf8JsonWriter writer, long? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteStringValue(value.Value.ToString());
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}

