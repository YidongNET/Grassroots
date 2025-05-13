# Grassroots Framework

Grassroots是一个基于.NET 8平台的现代化应用框架，采用领域驱动设计(DDD)架构，旨在为企业级应用提供一个高效、可扩展的基础架构。

## 核心目标

- 提供清晰的领域驱动设计架构
- 实现关注点分离，促进代码重用
- 支持可测试性和可维护性
- 集成现代化开发实践
- 提供开箱即用的基础设施

## 架构

Grassroots框架采用分层架构，每一层都有其特定的职责：

1. **用户界面层**（Grassroots.Api）：处理HTTP请求，提供API接口
2. **应用层**（Grassroots.Application）：协调业务流程，编排领域对象
3. **领域层**（Grassroots.Domain）：包含业务规则、领域模型和业务逻辑
4. **模型层**（Grassroots.Model）：定义实体和值对象
5. **基础设施层**（Grassroots.Infrastructure）：提供技术细节实现，如数据库访问、消息队列等

## 项目结构与依赖关系

### 项目概览

1. **Grassroots.Model** - 模型层
   - 包含DTO、实体定义和映射接口
   - 最基础的项目，不依赖其他项目
   - 目录结构：
     - `Dto/`: 数据传输对象
     - `Entities/`: 基础实体定义
     - `Mapping/`: 对象映射接口

2. **Grassroots.Domain** - 领域层
   - 包含领域实体、聚合根、值对象、领域事件和仓储接口
   - 依赖: `Grassroots.Model`
   - 目录结构：
     - `AggregateRoots/`: 聚合根定义
     - `Entities/`: 领域实体
     - `Entity/`: 实体基础类
     - `Events/`: 领域事件和事件处理器接口
     - `Repositories/`: 仓储接口
     - `ValueObjects/`: 值对象

3. **Grassroots.Application** - 应用层
   - 包含命令、查询、处理器和调度器接口
   - 依赖: `Grassroots.Domain`, `Grassroots.Model`
   - 目录结构：
     - `Commands/`: 命令和命令处理器接口
     - `Dispatchers/`: 命令和查询调度器接口
     - `Events/`: 集成事件和事件处理器
     - `Queries/`: 查询和查询处理器接口
     - `Logging/`: 日志扩展方法

4. **Grassroots.Infrastructure** - 基础设施层
   - 包含EF Core DbContext、仓储实现、命令和查询的具体实现、自动映射等
   - 依赖: `Grassroots.Application`, `Grassroots.Domain`, `Grassroots.Model`
   - 目录结构：
     - `Commands/`: 命令和命令处理器实现
     - `Data/`: 数据访问和DbContext
     - `DependencyInjection/`: 依赖注入配置
       - `AutofacModules/`: Autofac模块组织
         - `ApplicationModule.cs`: 应用层依赖注册
         - `DbModule.cs`: 数据库相关依赖注册
         - `InfrastructureModule.cs`: 基础设施服务注册
         - `ServiceDiscoveryModule.cs`: 服务发现组件注册
         - `LoggingModule.cs`: 日志服务注册
     - `Extensions/`: 服务注册扩展方法
     - `Events/`: 事件总线和事件存储实现
     - `Logging/`: 日志配置和助手类
     - `Mapping/`: 自动映射实现
     - `Migrations/`: 数据库迁移
     - `Queries/`: 查询处理器实现
     - `Repositories/`: 仓储实现
     - `ServiceDiscovery/`: 服务发现与注册
       - `ConsulOptions.cs`: Consul配置选项
       - `ConsulServiceDiscovery.cs`: Consul服务发现实现
       - `IServiceDiscovery.cs`: 服务发现接口
       - `ServiceDiscoveryHttpClientFactory.cs`: 基于服务发现的HTTP客户端工厂
       - `ServiceDiscoveryOptions.cs`: 服务发现选项和功能开关

5. **Grassroots.Api** - API层(用户界面层)
   - 包含控制器和程序入口点
   - 依赖: `Grassroots.Application`, `Grassroots.Infrastructure`, `Grassroots.Model`
   - 目录结构：
     - `Controllers/`: API控制器
       - `HealthController.cs`: 健康检查控制器
       - `ServiceDiscoveryDemoController.cs`: 服务发现演示
       - `LoggingDemoController.cs`: 日志功能演示
     - `Program.cs`: 应用程序入口点和配置

### 依赖关系图

```
Grassroots.Api
    ├── Grassroots.Application
    │       ├── Grassroots.Domain
    │       │       └── Grassroots.Model
    │       └── Grassroots.Model
    ├── Grassroots.Infrastructure
    │       ├── Grassroots.Application
    │       │       ├── Grassroots.Domain
    │       │       │       └── Grassroots.Model
    │       │       └── Grassroots.Model
    │       ├── Grassroots.Domain
    │       │       └── Grassroots.Model
    │       └── Grassroots.Model
    └── Grassroots.Model
```

``` 
                   +-----------------+
                   |  Grassroots.Api |
                   +-----------------+
                     /      |      \
                    /       |       \
                   /        |        \
                  /         |         \
   +--------------------+   |   +---------------------+
   | Grassroots.Application  |   | Grassroots.Infrastructure |
   +--------------------+   |   +---------------------+
              \             |            /     |
               \            |           /      |
                \           |          /       |
                 \          |         /        |
             +--------------------+  /         |
             |  Grassroots.Domain |  /         |
             +--------------------+ /          |
                        \          /           |
                         \        /            |
                          \      /             |
                           \    /              |
                            \  /               |
                      +-------------------+    |
                      |  Grassroots.Model |<---+
                      +-------------------+
``` 


### 模块组织

项目使用Autofac模块化组织依赖注入，主要模块包括：

1. **LoggingModule** - 日志模块
   - 注册Serilog日志服务
   - 配置日志收集和输出

2. **InfrastructureModule** - 基础设施模块
   - 注册命令和查询分发器
   - 注册AutoMapper适配器
   - 注册事件总线和事件处理相关服务
   - 注册通用仓储实现

3. **ApplicationModule** - 应用层模块
   - 注册命令处理器（ICommandHandler<T>和ICommandHandler<T,R>）
   - 注册查询处理器（IQueryHandler<T,R>）
   - 注册事件处理器（IDomainEventHandler<T>和IIntegrationEventHandler<T>）

4. **DbModule** - 数据库模块
   - 注册DbContext
   - 配置数据库提供程序（SQL Server/PostgreSQL/MySQL）
   - 注册实体特定的仓储实现

5. **ServiceDiscoveryModule** - 服务发现模块
   - 注册服务发现选项和配置
   - 根据配置决定是否启用服务发现功能
   - 注册Consul服务发现客户端和相关组件

## 依赖注入

### Autofac依赖注入容器

Grassroots框架使用Autofac作为依赖注入容器，提供更强大和灵活的依赖注入能力：

#### 核心组件：

- `LoggingModule` - 注册日志服务
- `InfrastructureModule` - 注册基础设施服务
- `ApplicationModule` - 注册应用层服务
- `ServiceDiscoveryModule` - 注册服务发现相关服务
- `DbModule` - 注册数据库相关服务
- `AutofacExtensions` - Autofac扩展方法

#### 注册模块：

```csharp
public class InfrastructureModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // 注册命令和查询分发器
        builder.RegisterType<CommandDispatcher>().As<ICommandDispatcher>().InstancePerLifetimeScope();
        builder.RegisterType<QueryDispatcher>().As<IQueryDispatcher>().InstancePerLifetimeScope();
        
        // 注册AutoMapper
        builder.RegisterAutoMapper();
        builder.RegisterType<AutoMapperAdapter>().As<IMapperInterface>().SingleInstance();
        
        // 注册事件相关服务
        builder.RegisterType<DomainEventBus>().As<IDomainEventBus>().SingleInstance();
        builder.RegisterType<IntegrationEventBus>().As<IIntegrationEventBus>().SingleInstance();
        builder.RegisterType<EventStore>().As<IEventStore>().InstancePerLifetimeScope();
        builder.RegisterType<EventMediator>().As<IEventMediator>().InstancePerLifetimeScope();
    }
}
```

#### 配置Autofac：

在`Program.cs`中配置Autofac：

```csharp
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(builder =>
    {
        builder.RegisterModule(new LoggingModule(configuration));
        builder.RegisterModule(new InfrastructureModule());
        builder.RegisterModule(new ApplicationModule());
        builder.RegisterModule(new ServiceDiscoveryModule());
        builder.RegisterModule(new DbModule(configuration));
    });
```

## 特性

- 基于.NET 8平台
- 遵循领域驱动设计(DDD)原则
- 采用命令查询职责分离(CQRS)模式
- 支持依赖注入（Autofac）
- 提供通用仓储模式实现
- RESTful API支持
- OpenAPI/Swagger集成
- 强类型配置
- 微服务架构支持
- 服务注册与发现
- 结构化日志记录（Serilog）

## 日志系统

Grassroots框架使用Serilog实现了结构化日志系统，支持多种日志输出目标和灵活的配置选项。

### 日志功能特点

- 使用Serilog实现结构化日志记录
- 支持控制台和文件输出
- 支持请求和响应日志记录
- 提供日志级别控制
- 支持日志开关功能
- 日志消息丰富化（线程ID、环境信息等）
- 支持不同环境的差异化日志配置

### 日志配置

在`appsettings.json`文件中，您可以配置日志系统：

```json
{
  "Serilog": {
    "Enabled": true,
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning",
        "Microsoft.AspNetCore": "Warning",
        "Microsoft.EntityFrameworkCore": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/log-.txt",
          "rollingInterval": "Day",
          "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}",
          "fileSizeLimitBytes": 10485760,
          "retainedFileCountLimit": 30
        }
      }
    ],
    "Enrich": [ "FromLogContext", "WithMachineName", "WithThreadId" ],
    "Properties": {
      "Application": "Grassroots"
    }
  }
}
```

### 使用日志

框架提供了扩展方法简化日志使用:

```csharp
// 注入ILogger
private readonly ILogger<MyClass> _logger;

// 使用标准日志方法
_logger.LogInformation("这是一条信息日志");
_logger.LogWarning("这是一条警告日志");
_logger.LogError(exception, "发生了错误");

// 使用扩展方法
_logger.LogAppInfo("这是一条应用信息日志");
_logger.LogAppWarning("这是一条应用警告日志");
_logger.LogAppError(exception, "发生了应用错误");

// 使用结构化日志
_logger.LogInformation("用户 {UserId} 执行了 {Action}", userId, action);
```

### 日志开关控制

可以通过API动态控制日志开关:

```
GET /api/LoggingDemo/status - 获取日志状态
POST /api/LoggingDemo/toggle?enabled=true - 启用日志
POST /api/LoggingDemo/toggle?enabled=false - 禁用日志
```

## 数据访问

Grassroots框架使用Entity Framework Core 8作为ORM工具，并支持多种数据库系统：

- SQL Server
- PostgreSQL
- MySQL

### 配置数据库提供程序

在`appsettings.json`文件中，您可以轻松切换数据库提供程序：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "您的连接字符串"
  },
  "Database": {
    "ProviderType": "SqlServer" // 可选值: SqlServer, PostgreSQL, MySQL
  }
}
```