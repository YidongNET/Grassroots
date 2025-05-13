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
     - `Events/`: 领域事件
     - `Repositories/`: 仓储接口
     - `ValueObjects/`: 值对象

3. **Grassroots.Application** - 应用层
   - 包含命令、查询、处理器和调度器接口
   - 依赖: `Grassroots.Domain`, `Grassroots.Model`
   - 目录结构：
     - `Commands/`: 命令和命令处理器接口
     - `Dispatchers/`: 命令和查询调度器接口
     - `Queries/`: 查询和查询处理器接口

4. **Grassroots.Infrastructure** - 基础设施层
   - 包含EF Core DbContext、仓储实现、命令和查询的具体实现、自动映射等
   - 依赖: `Grassroots.Application`, `Grassroots.Domain`, `Grassroots.Model`
   - 目录结构：
     - `Commands/`: 命令实现
     - `Data/`: 数据访问和DbContext
     - `DependencyInjection/`: 依赖注入配置
     - `Extensions/`: 服务注册扩展方法
     - `Mapping/`: 自动映射实现
     - `Migrations/`: 数据库迁移
     - `Queries/`: 查询实现
     - `Repositories/`: 仓储实现
     - `ServiceDiscovery/`: 服务发现与注册
     - `Events/`: 事件相关实现

5. **Grassroots.Api** - API层(用户界面层)
   - 包含控制器和程序入口点
   - 依赖: `Grassroots.Application`, `Grassroots.Infrastructure`, `Grassroots.Model`
   - 目录结构：
     - `Controllers/`: API控制器
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

## 依赖注入

### Autofac依赖注入容器

Grassroots框架使用Autofac作为依赖注入容器，提供更强大和灵活的依赖注入能力：

#### 核心组件：

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

### 数据库迁移

创建迁移：

```bash
cd Grassroots.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../Grassroots.Api
```

更新数据库：

```bash
cd Grassroots.Infrastructure
dotnet ef database update --startup-project ../Grassroots.Api
```

## 微服务架构

Grassroots框架提供了对微服务架构的支持，包括以下功能：

### 服务注册与发现

框架集成了Consul作为服务注册与发现中心，允许微服务自动注册并发现其他服务。

#### 核心组件：

- `IServiceDiscovery` - 服务发现接口
- `ConsulServiceDiscovery` - Consul服务发现实现
- `ServiceDiscoveryHttpClientFactory` - 基于服务发现的HTTP客户端工厂
- `ServiceInstance` - 服务实例模型
- `ServiceDiscoveryOptions` - 服务发现配置选项
- `FeaturesOptions` - 功能开关配置选项

#### 服务注册配置：

在`appsettings.json`文件中，您可以配置服务和Consul连接信息：

```json
{
  "Service": {
    "Id": "",
    "Name": "GrassrootsService",
    "Address": "localhost",
    "Port": 5000,
    "Tags": [ "api", "grassroots", "ddd" ]
  },
  "Consul": {
    "Enabled": true,
    "Address": "http://localhost:8500",
    "HealthCheck": true,
    "HealthCheckPath": "/health",
    "HealthCheckInterval": 10,
    "HealthCheckTimeout": 5
  },
  "Features": {
    "ServiceDiscovery": true
  }
}
```

#### 服务开关功能：

框架支持通过配置文件控制服务发现功能的开启和关闭：

- `Consul.Enabled` - 控制Consul服务是否启用
- `Features.ServiceDiscovery` - 控制服务发现功能是否启用

在开发环境中，您可以在`appsettings.Development.json`中默认禁用服务发现：

```json
{
  "Features": {
    "ServiceDiscovery": false
  }
}
```

#### 健康检查：

框架提供了内置的健康检查端点，Consul会通过这个端点检查服务的健康状态：

- `/health` - 健康检查端点

#### 服务调用：

使用`ServiceDiscoveryHttpClientFactory`可以方便地调用其他微服务：

```csharp
// 注入工厂
private readonly ServiceDiscoveryHttpClientFactory _httpClientFactory;

// 调用服务
var result = await _httpClientFactory.ExecuteRequestAsync<JsonDocument>(
    "OtherService", 
    async (client) => {
        var response = await client.GetAsync("/api/resource");
        response.EnsureSuccessStatusCode();
        return JsonDocument.Parse(await response.Content.ReadAsStringAsync());
    });
```

## 事件架构

项目实现了两种类型的事件：

1. **领域事件（Domain Events）** - 在单个限界上下文内部发生的事件
2. **集成事件（Integration Events）** - 在不同限界上下文之间或与外部系统共享的事件

### 领域事件

领域事件代表领域中发生的重要事情。它们用于：

- 解耦领域逻辑
- 在限界上下文内实现最终一致性
- 支持事件溯源

#### 核心组件：

- `DomainEvent` - 所有领域事件的基类
- `IDomainEventHandler<T>` - 领域事件处理器接口
- `IDomainEventBus` - 发布领域事件的接口
- `DomainEventBus` - 领域事件总线实现

### 集成事件

集成事件用于促进限界上下文之间或与外部系统的通信。它们用于：

- 跨限界上下文通信
- 与外部系统集成
- 在整个系统中实现最终一致性

#### 核心组件：

- `IntegrationEvent` - 所有集成事件的基类
- `IIntegrationEventHandler<T>` - 集成事件处理器接口
- `IIntegrationEventBus` - 发布集成事件的接口
- `IntegrationEventBus` - 集成事件总线实现

### 事件溯源

系统支持事件溯源，这允许：

- 从聚合根的事件历史中重建其状态
- 时间旅行调试
- 审计日志记录

#### 核心组件：

- `IEventStore` - 事件存储接口
- `EventStore` - 事件存储实现
- `EventStoreEntity` - 存储事件的实体
- 增强的 `AggregateRoot` 基类以支持事件溯源

### 发布-订阅模型

实现了发布-订阅模型用于事件分发：

- `EventMediator` - 事件中介者
- 事件序列化/反序列化
- 依赖注入配置

## 示例实现

项目包含以下示例：

- 领域事件实现
- 集成事件实现
- 事件处理器
- 服务发现演示控制器
- 健康检查控制器

## 运行说明

### 前提条件

- .NET 8 SDK
- Visual Studio 2022或其他兼容IDE
- SQL Server/PostgreSQL/MySQL（根据您的选择）
- Consul（用于服务发现，可选）

### 克隆仓库

```bash
git clone https://github.com/YidongNET/Grassroots.git
cd Grassroots
```

### 命令行运行

```bash
cd Grassroots.Api
dotnet run
```

### Visual Studio运行

1. 打开`Grassroots.sln`解决方案文件
2. 将`Grassroots.Api`设置为启动项目
3. 按`F5`运行项目

### 安装和运行Consul（可选）

如果需要服务注册与发现功能：

1. 下载并安装Consul: https://www.consul.io/downloads
2. 运行Consul开发模式:

```bash
consul agent -dev
```

3. 访问Consul UI: http://localhost:8500

## 贡献

欢迎提交问题和拉取请求，一起改进Grassroots框架！

## 许可

本项目采用MIT许可证。 