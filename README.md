# Grassroots

一个基于.NET 8框架的DDD领域驱动设计和洋葱架构的应用程序。

## 项目概述

Grassroots是一个使用领域驱动设计(DDD)和洋葱架构构建的现代化.NET 8应用程序。该项目展示了如何在实际开发中应用DDD的核心概念和洋葱架构的分层设计，为构建企业级应用提供了坚实的基础。

## 技术栈

- **.NET 8** - 基础框架
- **ASP.NET Core 8** - Web API框架
- **Entity Framework Core 8** - 多数据库ORM支持(SQL Server、PostgreSQL、MySQL)
- **Autofac** - 依赖注入容器
- **Serilog** - 结构化日志记录
- **Consul** - 服务注册与发现
- **Swagger** - API文档
- **System.Text.Json** - JSON序列化
- **AutoMapper** - 对象映射

## 如何开始

### 先决条件

- .NET 8 SDK
- Visual Studio 2022 或 Visual Studio Code
- SQL Server/PostgreSQL/MySQL (可选，取决于持久化方式)

### 构建和运行

1. 克隆仓库：
   ```
   git clone https://github.com/YidongNET/Grassroots.git
   ```

2. 导航到项目目录：
   ```
   cd Grassroots
   ```

3. 构建解决方案：
   ```
   dotnet build
   ```

4. 运行API项目：
   ```
   dotnet run --project Grassroots.Api
   ```

5. 在浏览器中访问：
   ```
   https://localhost:5111/swagger
   ```

## 依赖关系

项目遵循洋葱架构的依赖原则：

- API层依赖于应用层
- 应用层依赖于领域层和基础设施层
- 基础设施层依赖于领域层
- 领域层不依赖任何其他层



## 项目架构

项目采用领域驱动设计(DDD)和洋葱架构的组合，严格遵循洋葱架构的分层依赖原则：

- **领域层(Domain)** 处于核心位置，不依赖任何其他层
- **应用层(Application)** 依赖领域层，协调领域对象完成业务逻辑
- **基础设施层(Infrastructure)** 依赖领域层，提供技术实现
- **API层** 依赖应用层，处理用户交互

### 领域驱动设计(DDD)实现

- **实体(Entities)**: 具有唯一标识的对象
- **值对象(Value Objects)**: 无唯一标识的不可变对象
- **领域服务(Domain Services)**: 处理跨实体业务逻辑
- **聚合(Aggregates)**: 实体的边界集合
- **仓储(Repositories)**: 持久化实体的抽象
- **领域事件(Domain Events)**: 领域内部的通知机制
- **事件溯源(Event Sourcing)**: 通过事件记录实体状态变化

### 洋葱架构实现

从内到外的分层结构：

1. **领域层(Domain Layer)** - 核心业务逻辑和规则
2. **应用层(Application Layer)** - 协调领域对象完成用户操作
3. **基础设施层(Infrastructure Layer)** - 提供持久化、消息传递等技术支持
4. **表示层/API层(Presentation/API Layer)** - 处理用户请求和响应

## 解决方案结构

- **Grassroots.Domain** - 领域层
  - Entities - 实体与聚合根
    - `Entity<TKey>` - 通用实体基类，集成领域事件功能
    - `AggregateRoot<TKey>` - 聚合根基类，添加了审计字段
    - `EventSourcedAggregateRoot` - 支持事件溯源的聚合根
  - ValueObjects - 值对象
  - Events - 领域事件系统
    - `IDomainEvent` - 领域事件接口
    - `DomainEvent` - 领域事件基类
    - `IHasDomainEvents` - 支持领域事件的实体接口
  - Interfaces - 接口
    - `IRepository<T>` - 泛型仓储接口
    - `IUnitOfWork` - 工作单元接口
    - `IIdGenerator` - ID生成器接口
  - Services - 领域服务
  - Exceptions - 异常

- **Grassroots.Application** - 应用层
  - Features - 按功能组织的业务用例
  - Dtos - 数据传输对象
  - Interfaces - 应用服务接口
  - Services - 应用服务实现
  - Mappings - 对象映射配置
  - DependencyInjection - 依赖注入模块

- **Grassroots.Infrastructure** - 基础设施层
  - Data - 数据访问
    - `AppDbContext` - EF Core数据上下文
    - `RepositoryBase<T>` - 通用仓储实现
    - `UnitOfWork` - 工作单元实现 
    - `DbContextFactory` - 多数据库支持工厂
  - Events - 事件处理
    - `EventStore` - 事件存储实现
    - `DomainEventService` - 领域事件处理服务
    - `InMemoryEventBus` - 内存事件总线
  - IdGenerators - ID生成
    - `SnowflakeIdGenerator` - 雪花算法ID生成器
  - ServiceDiscovery - 服务发现
    - `ConsulServiceDiscovery` - Consul服务注册与发现实现
  - DependencyInjection - 依赖注入
    - `InfrastructureModule` - Autofac模块

- **Grassroots.Api** - API层
  - Controllers - 控制器
    - `HealthController` - 健康检查控制器
    - `IdGeneratorController` - ID生成器API示例
  - Filters - 过滤器
  - Program.cs - 应用启动配置

## 详细目录结构

项目采用层次分明的目录结构，清晰展示了领域驱动设计和洋葱架构的实现：

### 根目录
```
├── Grassroots.Domain/            # 领域层 - 核心业务逻辑
├── Grassroots.Application/       # 应用层 - 业务用例和协调
├── Grassroots.Infrastructure/    # 基础设施层 - 技术实现
├── Grassroots.Api/               # API层 - 用户接口
├── docs/                         # 文档目录
├── .github/                      # GitHub配置
├── Grassroots.sln                # 解决方案文件
├── README.md                     # 项目说明文档
└── LICENSE                       # 许可证文件
```

### 领域层 (Grassroots.Domain)
```
├── Entities/                     # 实体定义
│   ├── Entity.cs                 # 通用实体基类
│   ├── AggregateRoot.cs          # 聚合根基类
│   └── EventSourcedAggregateRoot.cs # 支持事件溯源的聚合根
├── Events/                       # 领域事件
│   ├── IDomainEvent.cs           # 领域事件接口
│   ├── DomainEvent.cs            # 领域事件基类
│   ├── IHasDomainEvents.cs       # 支持领域事件的实体接口
│   ├── IEventStore.cs            # 事件存储接口
│   ├── IDomainEventService.cs    # 领域事件服务接口
│   ├── IEventBus.cs              # 事件总线接口
│   ├── IIntegrationEvent.cs      # 集成事件接口
│   ├── IntegrationEvent.cs       # 集成事件基类
│   └── IIntegrationEventHandler.cs # 集成事件处理器接口
├── Interfaces/                   # 领域接口
│   ├── IRepository.cs            # 仓储接口
│   ├── IUnitOfWork.cs            # 工作单元接口
│   └── IIdGenerator.cs           # ID生成器接口
├── ValueObjects/                 # 值对象
├── Services/                     # 领域服务
├── Exceptions/                   # 领域异常
└── Enums/                        # 枚举定义
```

### 应用层 (Grassroots.Application)
```
├── Features/                     # 按功能组织的业务用例
├── Services/                     # 应用服务实现
├── Interfaces/                   # 应用服务接口
├── Dtos/                         # 数据传输对象
├── Mappings/                     # 对象映射配置
├── DependencyInjection/          # 依赖注入模块
└── Common/                       # 通用功能
```

### 基础设施层 (Grassroots.Infrastructure)
```
├── Data/                         # 数据访问
│   ├── AppDbContext.cs           # EF Core数据上下文
│   ├── RepositoryBase.cs         # 通用仓储实现
│   ├── UnitOfWork.cs             # 工作单元实现
│   └── DbContextFactory.cs       # 多数据库支持工厂
├── Events/                       # 事件处理
│   ├── EventStore.cs             # 事件存储实现
│   ├── DomainEventService.cs     # 领域事件服务
│   ├── InMemoryEventBus.cs       # 内存事件总线
│   ├── DomainToIntegrationEventMapper.cs # 事件映射器
│   └── DomainEventToIntegrationEventHandler.cs # 领域事件处理器
├── IdGenerators/                 # ID生成器
│   └── SnowflakeIdGenerator.cs   # 雪花算法ID生成器
├── ServiceDiscovery/             # 服务发现
├── Repositories/                 # 仓储实现
├── Persistence/                  # 持久化相关
├── Identity/                     # 身份认证
├── Logging/                      # 日志记录
├── Services/                     # 基础服务
├── DependencyInjection/          # 依赖注入
└── DependencyInjectionExtensions.cs # 依赖注入扩展方法
```

### API层 (Grassroots.Api)
```
├── Controllers/                  # API控制器
│   ├── HealthController.cs       # 健康检查控制器
│   └── IdGeneratorController.cs  # ID生成器API示例
├── Filters/                      # 过滤器
├── Logs/                         # 日志文件
├── Properties/                   # 项目属性
├── Program.cs                    # 应用程序入口和配置
├── appsettings.json              # 应用程序配置
└── appsettings.Development.json  # 开发环境配置
```

## 项目结构关系

Grassroots项目基于领域驱动设计(DDD)和洋葱架构实现，以下是各层之间的依赖关系和核心组件交互图：

### 层次依赖关系图

```
           ┌─────────────────────────────┐
           │                             │
           │     Grassroots.Api          │ 
           │    (表示层/接口层)           │
           │                             │
           └─────────────┬───────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────┐
│                                                         │
│               Grassroots.Application                    │
│                  (应用服务层)                           │
│                                                         │
└─────────────────┬──────────────────┬───────────────────┘
                  │                  │
                  ▼                  ▼
┌─────────────────────────┐ ┌─────────────────────────────┐
│                         │ │                             │
│  Grassroots.Domain      │ │  Grassroots.Infrastructure  │
│     (领域层)            │◄┘      (基础设施层)           │
│                         │ │                             │
└─────────────────────────┘ └─────────────────────────────┘
```

### 依赖关系说明

1. **领域层(Grassroots.Domain)**
   - 位于核心位置，不依赖其他任何层
   - 定义领域模型、业务规则和核心接口
   - 包含实体、值对象、领域事件和领域服务

2. **应用层(Grassroots.Application)**
   - 依赖领域层
   - 协调领域对象实现用例
   - 实现应用服务，处理用户请求
   - 不包含业务规则，只负责协调

3. **基础设施层(Grassroots.Infrastructure)**
   - 依赖领域层，实现领域层定义的接口
   - 不依赖应用层
   - 提供技术实现：数据访问、消息队列、身份认证等

4. **API层(Grassroots.Api)**
   - 依赖应用层
   - 处理HTTP请求和响应
   - 不包含业务逻辑

### 核心组件交互关系

#### 领域事件流程

```
┌─────────────┐    ┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│             │    │             │    │             │    │             │
│  Entity     │───►│ DomainEvent │───►│EventService │───►│  EventBus   │
│             │    │             │    │             │    │             │
└─────────────┘    └─────────────┘    └─────────────┘    └─────────────┘
                                             │                  │
                                             ▼                  ▼
                                      ┌─────────────┐    ┌─────────────┐
                                      │             │    │             │
                                      │ EventStore  │    │ EventHandler│
                                      │             │    │             │
                                      └─────────────┘    └─────────────┘
```

#### 仓储与工作单元模式

```
┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│             │    │             │    │             │
│ AppService  │───►│ Repository  │───►│ UnitOfWork  │
│             │    │             │    │             │
└─────────────┘    └─────────────┘    └─────────────┘
                          │                  │
                          ▼                  ▼
                   ┌─────────────┐    ┌─────────────┐
                   │             │    │             │
                   │ DbContext   │◄───┤ Transaction │
                   │             │    │             │
                   └─────────────┘    └─────────────┘
```

#### 服务注册与发现

```
┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│             │    │             │    │             │
│  Program.cs │───►│ServiceDisc. │───►│   Consul    │
│             │    │             │    │             │
└─────────────┘    └─────────────┘    └─────────────┘
       │                                     │
       │                                     ▼
       │                             ┌─────────────┐
       │                             │             │
       └────────────────────────────►│Health Check │
                                     │             │
                                     └─────────────┘
```

### 业务流程示例

以ID生成器API为例说明完整请求流程：

1. **API层**：`IdGeneratorController` 接收HTTP请求
2. **应用层**：应用服务协调操作（简单示例中省略）
3. **领域层**：`IIdGenerator` 接口定义生成ID的契约
4. **基础设施层**：`SnowflakeIdGenerator` 实现ID生成算法
5. **响应**：生成的ID返回给客户端

### 横切关注点

* **依赖注入**：通过Autofac容器注册和解析依赖
* **日志记录**：使用Serilog实现结构化日志
* **配置管理**：使用.NET配置系统管理应用配置
* **错误处理**：统一的异常处理和HTTP响应

## 功能亮点

1. **完整的DDD实现**
   - 实体、值对象、聚合根等DDD概念的完整实现
   - 领域事件和事件溯源

2. **高度可配置的雪花算法ID生成器**
   - 支持自定义位结构配置
   - 随机WorkerId支持
   - long类型转string解决JavaScript精度问题

3. **多数据库支持**
   - 支持SQL Server、PostgreSQL、MySQL
   - 统一的数据访问接口
   - 简单的配置切换

4. **服务注册与发现**
   - Consul集成
   - 自动健康检查
   - 服务注册生命周期管理

5. **结构化日志**
   - Serilog集成
   - 控制台和文件输出
   - 上下文信息丰富
   - 可配置日志开关

6. **依赖注入增强**
   - Autofac替代默认容器
   - 模块化注册
   - 更灵活的生命周期管理

7. **统一RESTful API响应规范**
   - 所有API接口返回统一的JSON结构
   - 全局异常处理，所有错误均返回标准JSON格式

## API响应规范

所有API接口响应均采用统一的RESTful风格JSON结构，示例：

```
{
  "success": true,
  "code": 200,
  "message": "Success",
  "data": { ... },
  "timestamp": 1688888888888
}
```

错误响应示例：

```
{
  "success": false,
  "code": 400,
  "message": "参数错误",
  "data": null,
  "timestamp": 1688888888888
}
```

### 说明
- `success`：请求是否成功
- `code`：HTTP状态码
- `message`：提示信息
- `data`：返回数据内容，错误时为null
- `timestamp`：响应时间戳（毫秒）

### 全局异常处理
项目内置全局异常处理中间件，所有未捕获异常均返回统一JSON格式，便于前端统一处理。

## 配置特性

项目使用标准的.NET配置系统，主要配置包括：

```json
{
  "Database": {
    "ProviderType": "SqlServer"  // 可选：SqlServer, PostgreSQL, MySQL
  },
  "Snowflake": {
    "DatacenterId": 1,
    "WorkerId": -1,  // -1表示自动生成随机WorkerId
    "SequenceBits": 12,
    "WorkerIdBits": 5,
    "DatacenterIdBits": 5
  },
  "Serilog": {
    "Enabled": true,  // 可切换开关
    // 各种日志级别和输出配置
  },
  "Consul": {
    "Enabled": true,  // 可切换开关
    // 服务发现配置
  }
}
```



## 许可证

[MIT](LICENSE) 
