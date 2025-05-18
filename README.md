# Grassroots Framework

Grassroots是一个基于.NET 8平台的现代化应用框架，采用依赖倒置原则(DIP)和洋葱架构实现，旨在为企业级应用提供一个高效、可扩展的基础架构。

## 核心目标

- 提供清晰的领域驱动设计架构
- 实现关注点分离，促进代码重用
- 支持可测试性和可维护性
- 集成现代化开发实践
- 提供开箱即用的基础设施


## 项目架构

项目采用四层架构，每一层都有明确的职责：

- **Domain层**：包含业务实体、值对象、领域事件和仓储接口
- **Application层**：包含业务用例、命令/查询处理程序(CQRS)和应用服务接口
- **Infrastructure层**：包含数据访问、外部服务集成和技术实现
- **API层**：提供REST API接口，处理HTTP请求和响应

### 依赖关系

项目遵循严格的依赖规则：

```
API层 → Application层 → Domain层 ← Infrastructure层
                          ↑
                    Infrastructure层
```

- **Domain层**：不依赖任何其他层
- **Application层**：仅依赖Domain层
- **Infrastructure层**：依赖Domain层和Application层
- **API层**：仅依赖Application层，不直接依赖Infrastructure层

## 依赖倒置原则(DIP)实现

本项目严格实现了依赖倒置原则：

1. **抽象定义在Domain和Application层**
   - `IRepository<T>`等接口定义在Domain层
   - `IApplicationDbContext`等接口定义在Application层

2. **实现在Infrastructure层**
   - Repository和DbContext等具体实现在Infrastructure层
   - 实现依赖抽象，抽象不依赖实现

3. **运行时动态加载**
   - API层通过反射动态加载Infrastructure层组件
   - 编译时完全解耦，运行时动态集成

4. **Autofac依赖注入**
   - 使用Autofac模块化管理依赖注册
   - 分层注册确保每层只访问允许的依赖

## 多数据库支持

项目支持多种数据库系统，无需修改业务代码：

- SQL Server
- PostgreSQL
- MySQL
- SQLite
- 内存数据库 (开发测试用)

通过`DatabaseFactory`工厂模式实现数据库提供程序的动态选择，配置采用简化方式，位于`appsettings.json`中：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "您的连接字符串"
  },
  "Database": {
    "ProviderType": "SqlServer" // 支持 SqlServer, PostgreSQL, MySQL, SQLite, InMemory
  }
}
```

切换数据库只需修改配置文件中的`ProviderType`值和相应的连接字符串，无需修改代码。系统会根据指定的提供程序类型使用对应的数据库技术。

## JSON序列化处理

项目实现了全局JSON序列化配置，解决JavaScript中处理大整数和高精度小数时的精度问题：

- **长整型自动转字符串**：所有long/ulong类型在JSON序列化时自动转为字符串格式
- **高精度小数处理**：decimal类型在JSON序列化时自动转为字符串格式
- **可空类型支持**：完全支持可空值类型的处理
- **双向转换**：支持从字符串反序列化回原始数值类型

这种处理方式的优势：

1. 避免了JavaScript中Number类型的精度限制(±2^53)
2. 确保前端获取的金额、ID等关键数值不会失真
3. 无需在每个模型属性上单独添加特性，全局自动处理

全局配置位于`Program.cs`中，通过自定义`LongToStringConverter`实现：

```csharp
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.Converters.Add(new LongToStringConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });
```

## 雪花算法（分布式ID生成）

项目集成了雪花算法（Snowflake）分布式ID生成器：

- **接口抽象**：`IIdGenerator`定义在Application层
- **具体实现**：`SnowflakeIdGenerator`实现在Infrastructure层
- **高度可配置**：支持通过配置自定义工作节点ID、数据中心ID和其他参数
- **时钟回拨处理**：包含时钟回拨的安全处理机制

配置示例：

```json
"Snowflake": {
  "DatacenterId": 1,
  "WorkerId": 1,
  "Epoch": 1672531200000, // 2023-01-01 作为起始时间戳
  "SequenceBits": 12,
  "WorkerIdBits": 5,
  "DatacenterIdBits": 5
}
```

使用方式：通过依赖注入获取`IIdGenerator`接口并调用`NextId()`方法即可获得全局唯一ID。

## Consul服务注册与发现

项目集成了Consul服务注册与发现功能：

- **可配置开关**：支持通过配置文件开启/关闭Consul服务
- **自动注册/注销**：应用启动时自动注册，关闭时自动注销
- **健康检查**：提供健康检查端点和自动检查机制
- **服务发现**：支持服务发现和负载均衡

配置示例：

```json
"Consul": {
  "Enabled": true, // 控制开关
  "ServiceName": "grassroots-api",
  "ServiceId": "grassroots-api-1",
  "ServiceAddress": "localhost",
  "ServicePort": 5000,
  "ConsulAddress": "http://localhost:8500",
  "HealthCheck": "/health",
  "Tags": ["api", "grassroots", "ddd"],
  "Interval": 10
}
```

使用方式：配置开启后，应用会自动向Consul注册；可通过依赖注入`IServiceDiscovery`接口发现和调用其他服务。

## Serilog结构化日志

项目集成了Serilog结构化日志系统：

- **可配置开关**：支持通过配置文件开启/关闭详细日志
- **多输出目标**：支持控制台、文件等多种输出方式
- **异步写入**：使用异步日志写入提高性能
- **结构化数据**：支持结构化日志数据便于分析
- **环境敏感**：针对不同环境提供不同的日志级别配置

配置示例：

```json
"Serilog": {
  "Enabled": true,
  "MinimumLevel": {
    "Default": "Information",
    "Override": {
      "Microsoft": "Warning",
      "System": "Warning"
    }
  },
  "WriteTo": [
    { "Name": "Console" },
    { 
      "Name": "File", 
      "Args": { 
        "path": "Logs/log-.txt",
        "rollingInterval": "Day"
      }
    }
  ]
}
```

## 事件系统

项目实现了完整的事件处理系统：

- **领域事件**：支持实体内的领域事件发布和处理
- **集成事件**：支持跨服务边界的集成事件
- **事件溯源**：支持基于事件溯源的聚合根实现
- **发布订阅**：内置发布-订阅模式实现

## 项目结构

```
Grassroots/
├── Grassroots.Domain/                # 领域层
│   ├── Entities/                     # 实体
│   │   └── BaseEntity.cs             # 实体基类
│   ├── ValueObjects/                 # 值对象
│   │   └── ValueObject.cs            # 值对象基类
│   ├── Events/                       # 领域事件
│   │   └── IDomainEvent.cs           # 领域事件接口
│   └── Repositories/                 # 仓储接口
│       └── IRepository.cs            # 通用仓储接口
│
├── Grassroots.Application/           # 应用层
│   ├── Common/                       # 通用组件
│   │   ├── Behaviors/                # MediatR行为
│   │   │   └── ValidationBehavior.cs # 验证行为
│   │   ├── Interfaces/               # 应用接口
│   │   │   └── IApplicationDbContext.cs # 数据库上下文接口
│   │   └── Models/                   # 应用模型
│   │       └── Result.cs             # 通用结果对象
│   ├── AutofacModules/               # Autofac模块
│   │   └── ApplicationModule.cs      # 应用层模块
│   └── DependencyInjection.cs        # 应用层依赖注入
│
├── Grassroots.Infrastructure/        # 基础设施层
│   ├── Persistence/                  # 持久化
│   │   ├── ApplicationDbContext.cs   # EF Core数据库上下文
│   │   └── DatabaseFactory.cs        # 数据库工厂 (支持多数据库)
│   ├── Repositories/                 # 仓储实现
│   │   └── Repository.cs             # 通用仓储实现
│   ├── Services/                     # 服务实现
│   │   ├── DomainEventService.cs     # 领域事件服务
│   │   └── SnowflakeIdGenerator.cs   # 雪花算法ID生成器
│   ├── EventBus/                     # 事件总线
│   │   └── InMemoryEventBus.cs       # 内存事件总线
│   ├── EventSourcing/                # 事件溯源
│   │   └── EventStore.cs             # 事件存储
│   ├── ServiceDiscovery/             # 服务发现
│   │   └── ConsulServiceDiscovery.cs # Consul服务发现
│   ├── AutofacModules/               # Autofac模块
│   │   └── InfrastructureModule.cs   # 基础设施层模块
│   └── DependencyInjection.cs        # 基础设施层依赖注入
│
└── Grassroots.Api/                   # API层
    ├── Converters/                   # JSON转换器
    │   └── LongToStringConverter.cs  # 长整型转字符串转换器
    ├── Extensions/                   # 扩展方法
    │   ├── ConsulExtensions.cs       # Consul扩展
    │   └── ServiceCollectionExtensions.cs # 服务集合扩展
    ├── Program.cs                    # 应用程序入口
    └── appsettings.json              # 应用配置
```

## 详细依赖关系分析

### 各层详细依赖关系

#### Domain层 (核心领域层)

**核心组件**:
- `AggregateRoot`: 聚合根基类
- `EventSourcedAggregateRoot`: 事件溯源聚合根
- `IDomainEvent`: 领域事件接口
- `IIntegrationEvent`: 集成事件接口

**包依赖**:
- `MediatR (12.2.0)`: 实现领域事件的发布/订阅和CQRS模式

#### Application层 (应用服务层)

**核心组件**:
- `IApplicationDbContext`: 数据库上下文接口
- `IDomainEventService`: 领域事件服务接口
- `IEventBus`: 事件总线接口
- `IEventStore`: 事件存储接口
- `IIdGenerator`: ID生成器接口
- MediatR行为: 领域事件分发、验证等

**包依赖**:
- `MediatR (12.2.0)`: 命令和查询处理
- `Autofac (7.1.0)`: IoC容器
- `Microsoft.Extensions.DependencyInjection.Abstractions (8.0.0)`: DI抽象

**项目依赖**:
- `Grassroots.Domain`: 依赖领域层定义的实体和事件

#### Infrastructure层 (基础设施层)

**核心组件**:
- `ApplicationDbContext`: EF Core上下文实现
- `DatabaseFactory`: 多数据库支持
- `EventStore`: 事件存储实现
- `InMemoryEventBus`: 内存事件总线
- `Repository`: 仓储实现
- `SnowflakeIdGenerator`: 雪花算法ID生成器
- `ConsulServiceDiscovery`: 服务发现实现

**包依赖**:
- 数据库相关:
  - `Microsoft.EntityFrameworkCore (8.0.1)`: ORM框架
  - `Microsoft.EntityFrameworkCore.SqlServer (8.0.1)`: SQL Server支持
  - `Npgsql.EntityFrameworkCore.PostgreSQL (8.0.0)`: PostgreSQL支持
  - `Pomelo.EntityFrameworkCore.MySql (8.0.0)`: MySQL支持
  - `Microsoft.EntityFrameworkCore.Sqlite (8.0.1)`: SQLite支持
  - `Microsoft.EntityFrameworkCore.InMemory (8.0.1)`: 内存数据库
  
- IoC相关:
  - `Autofac (7.1.0)`: IoC容器
  - `Autofac.Extensions.DependencyInjection (8.0.0)`: 与ASP.NET Core集成

- 日志相关:
  - `Serilog (3.1.1)`: 结构化日志
  - `Serilog.Enrichers.Environment (2.3.0)`: 环境信息
  - `Serilog.Enrichers.Thread (3.1.0)`: 线程信息
  - `Serilog.Sinks.Console (5.0.1)`: 控制台输出
  - `Serilog.Sinks.File (5.0.0)`: 文件输出

- 服务发现:
  - `Consul (1.7.14.1)`: 服务注册与发现

- 配置相关:
  - `Microsoft.Extensions.Configuration.Abstractions (8.0.0)`
  - `Microsoft.Extensions.Configuration.Binder (8.0.0)`
  - `Microsoft.Extensions.Options (8.0.0)`

**项目依赖**:
- `Grassroots.Domain`: 实现领域接口
- `Grassroots.Application`: 实现应用层接口

#### API层 (接口层)

**核心组件**:
- `Program.cs`: 应用启动配置
- `LongToStringConverter`: JSON转换器
- 各种Controller: REST API接口

**包依赖**:
- API相关:
  - `Microsoft.AspNetCore.OpenApi (8.0.0)`: OpenAPI规范
  - `Swashbuckle.AspNetCore (6.5.0)`: Swagger UI

- IoC相关:
  - `Autofac (7.1.0)`: IoC容器  
  - `Autofac.Extensions.DependencyInjection (8.0.0)`: 与ASP.NET Core集成

- 日志相关:
  - `Serilog.AspNetCore (8.0.0)`: ASP.NET Core集成
  - `Serilog.Enrichers.Environment (2.3.0)`: 环境信息
  - `Serilog.Enrichers.Thread (3.1.0)`: 线程信息
  - `Serilog.Expressions (4.0.0)`: 表达式支持
  - `Serilog.Settings.Configuration (8.0.0)`: 配置支持
  - `Serilog.Sinks.Async (1.5.0)`: 异步写入
  - `Serilog.Sinks.Console (5.0.1)`: 控制台输出
  - `Serilog.Sinks.File (5.0.0)`: 文件输出

**项目依赖**:
- `Grassroots.Domain`: 使用领域模型
- `Grassroots.Application`: 调用应用服务
- `Grassroots.Infrastructure`: 运行时通过反射加载实现

### 关键接口与实现关系

#### 数据访问层
- **接口**: `IApplicationDbContext` (Application层)
- **实现**: `ApplicationDbContext` (Infrastructure层)
- **依赖链**: `API` → `ApplicationDbContext` → `IApplicationDbContext`

#### 仓储模式
- **接口**: `IRepository<T>` (Domain层)
- **实现**: `Repository<T>` (Infrastructure层) 
- **依赖链**: `ApplicationService` → `IRepository<T>` → `Repository<T>`

#### 领域事件
- **接口**: `IDomainEventService` (Application层)
- **实现**: `DomainEventService` (Infrastructure层)
- **依赖链**: `AggregateRoot` → `IDomainEvent` → `IDomainEventService` → `DomainEventService`

#### 事件总线
- **接口**: `IEventBus` (Application层)
- **实现**: `InMemoryEventBus` (Infrastructure层)
- **依赖链**: `ApplicationService` → `IEventBus` → `InMemoryEventBus`

#### 事件存储
- **接口**: `IEventStore` (Application层)
- **实现**: `EventStore` (Infrastructure层)
- **依赖链**: `EventSourcedAggregateRoot` → `IEventSourcedAggregate` → `IEventStore` → `EventStore`

#### ID生成
- **接口**: `IIdGenerator` (Application层)
- **实现**: `SnowflakeIdGenerator` (Infrastructure层)
- **依赖链**: `Entity` → `IIdGenerator` → `SnowflakeIdGenerator`

#### 服务发现
- **接口**: `IServiceDiscovery` (Application层)
- **实现**: `ConsulServiceDiscovery` (Infrastructure层)
- **依赖链**: `Controller` → `IServiceDiscovery` → `ConsulServiceDiscovery`

### 依赖注入机制

#### 注册流程
1. **Application层**:
   - `DependencyInjection.cs`: 注册MediatR和应用服务
   - `ApplicationModule.cs`: Autofac模块配置

2. **Infrastructure层**:
   - `DependencyInjection.cs`: 注册具体实现
   - 各子系统模块注册

3. **API层**:
   - `Program.cs`: 通过反射动态加载Infrastructure层
   ```csharp
   var infrastructureAssembly = Assembly.Load("Grassroots.Infrastructure");
   var infrastructureDIType = infrastructureAssembly.GetType("Grassroots.Infrastructure.DependencyInjection");
   var registerMethod = infrastructureDIType?.GetMethod("RegisterInfrastructureServices");
   registerMethod?.Invoke(null, new object[] { containerBuilder, builder.Configuration });
   ```

#### 依赖倒置示例
1. **定义接口** (Application层):
   ```csharp
   public interface IEventBus
   {
       Task PublishAsync(IIntegrationEvent @event);
       void Subscribe<T, TH>() where T : IIntegrationEvent where TH : IIntegrationEventHandler<T>;
       void Unsubscribe<T, TH>() where T : IIntegrationEvent where TH : IIntegrationEventHandler<T>;
   }
   ```

2. **实现接口** (Infrastructure层):
   ```csharp
   public class InMemoryEventBus : IEventBus
   {
       // 实现方法...
   }
   ```

3. **使用接口** (Application层):
   ```csharp
   public class OrderService
   {
       private readonly IEventBus _eventBus;
       
       public OrderService(IEventBus eventBus)
       {
           _eventBus = eventBus;
       }
       
       // 使用_eventBus...
   }
   ```

### 数据库提供程序系统

#### 组件关系
- `DatabaseFactory`: 工厂类，根据配置创建不同数据库提供程序
- `ApplicationDbContext`: 使用工厂创建的提供程序
- `appsettings.json`: 配置数据库类型和连接字符串

#### 依赖关系
```
Program.cs (加载配置) → DatabaseFactory → ApplicationDbContext → Entity 映射
```

### 事件系统详细依赖

#### 领域事件流程
1. `AggregateRoot`: 添加领域事件
2. `DomainEventDispatcherBehavior`: MediatR行为拦截处理
3. `IDomainEventService`: 分发事件到处理器
4. `IDomainEventHandler<T>`: 处理特定类型事件

#### 集成事件流程
1. `IDomainEventHandler<T>`: 可能发布集成事件
2. `IEventBus`: 发布集成事件
3. `IIntegrationEventHandler<T>`: 处理集成事件

#### 事件溯源流程
1. `EventSourcedAggregateRoot`: 创建事件并应用状态变更
2. `IEventStore`: 保存事件到存储
3. `EventDescriptor`: 事件存储实体
4. `LoadFromHistory`: 重建聚合状态

## 技术栈

- **.NET 8**: 最新的.NET平台
- **Entity Framework Core**: ORM框架
- **MediatR**: CQRS和中介者模式实现
- **Autofac**: 依赖注入容器
- **多数据库支持**: SQL Server, PostgreSQL, MySQL, SQLite
- **Swagger/OpenAPI**: API文档生成
- **Serilog**: 结构化日志
- **Consul**: 服务注册与发现
- **雪花算法**: 分布式ID生成

## 如何运行

1. 克隆仓库
2. 确保已安装.NET 8 SDK
3. 在项目根目录执行以下命令：

```bash
# 还原依赖
dotnet restore

# 编译项目
dotnet build

# 运行API项目
dotnet run --project Grassroots.Api
```

默认情况下，API将在`http://localhost:5111`上运行。

## 数据库配置

项目默认使用SQL Server LocalDB。数据库设置在`Grassroots.Api/appsettings.json`中配置：

```json
"Database": {
  "ProviderType": "SqlServer",
  "SqlServerConnectionString": "Server=(localdb)\\mssqllocaldb;Database=GrassrootsDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

要切换到不同的数据库，只需修改`ProviderType`值。确保对应的数据库驱动程序已正确安装。

## 项目特点

- **严格的DDD架构**: 遵循领域驱动设计原则
- **CQRS模式**: 使用MediatR实现命令和查询分离
- **依赖倒置**: 高层不依赖低层实现，依赖于抽象
- **领域核心**: 领域层是项目核心，不依赖任何外部框架
- **模块化**: 使用Autofac实现模块化依赖管理
- **数据库无关**: 支持多种数据库系统，业务代码完全独立于数据库选择
- **可测试性**: 架构设计便于单元测试和集成测试
- **分布式支持**: 集成雪花算法和服务发现，支持分布式部署
- **可观测性**: 结构化日志和健康检查，支持系统监控与诊断 