using Grassroots.Infrastructure.Data;
using Grassroots.Infrastructure.DependencyInjection;
using Grassroots.Infrastructure.Extensions;
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

var builder = WebApplication.CreateBuilder(args);

// 配置使用Autofac作为DI容器
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    // 注册所有Grassroots模块
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

    // 启用XML注释
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);

    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// 添加服务发现
builder.Services.AddServiceDiscovery(builder.Configuration);

// 添加健康检查
builder.Services.AddHealthChecks();

var app = builder.Build();

// 初始化数据库
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

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// 使用服务注册（包含健康检查终结点）
app.UseServiceRegistration(app.Lifetime, builder.Configuration);

app.Run();
