# Grassroots框架

Grassroots框架是基于.NET 8平台开发的现代化应用框架，旨在支持企业级应用开发，提供高性能、高扩展性和模块化的解决方案。框架核心采用领域驱动设计（DDD）架构，以确保复杂业务逻辑能够得到清晰地表达和实现。

## 核心目标

1. **高效开发**：通过提供领域驱动的结构和基础设施支持，减少开发者的重复工作。
2. **可维护性**：采用分层架构和清晰的领域模型，降低代码复杂度，提升代码可读性和可维护性。
3. **扩展性**：支持分布式系统设计，易于集成第三方服务和扩展功能。
4. **现代化技术栈**：充分利用.NET 8的新特性（如性能优化、Minimal APIs和增强的原生支持）。

## 框架核心结构

### 1. 用户界面层（Presentation Layer）

- 提供API和前端交互接口
- 支持RESTful和gRPC服务
- 使用Minimal API提供轻量化的Web接口实现

### 2. 应用层（Application Layer）

- 负责处理用户请求，协调领域层完成业务逻辑
- 实现CQRS模式（Command和Query分离）
- 提供事务性和非事务性操作的支持

### 3. 领域层（Domain Layer）

- 核心业务逻辑所在地
- 包括聚合根、实体、值对象、领域服务等概念
- 遵循DDD原则，聚焦业务语义

### 4. 模型层（Model Layer）

- 负责封装应用程序中所需的所有数据结构和业务模型
- 包括领域层的实体、值对象及其关系，也可以包括应用层需要的投影模型
- 提供数据模型的转换与映射功能，支持应用层与领域层之间的数据传递和隔离
- 使用AutoMapper简化数据模型的转换与映射
- 确保业务逻辑的清晰表达，避免将业务逻辑混杂于数据模型中，提供良好的解耦

### 5. 基础设施层（Infrastructure Layer）

- 负责数据持久化、外部服务的集成等
- 使用.NET 8中的EF Core进行数据访问
- 支持分布式缓存（如Redis）、消息队列（如RabbitMQ或Kafka）

## 项目结构

- **Grassroots.Api**：API项目，提供接口和控制器
- **Grassroots.Application**：应用层，实现命令和查询处理
- **Grassroots.Domain**：领域层，包含业务实体和领域逻辑
- **Grassroots.Model**：模型层，包含数据传输对象和映射
- **Grassroots.Infrastructure**：基础设施层，提供数据访问和外部服务集成

## 详细目录结构

```
Grassroots/
│
├── Grassroots.Api/                   # 用户界面层
│   ├── Controllers/                  # API控制器
│   │   └── ApiBaseController.cs      # 控制器基类
│   ├── Program.cs                    # 应用程序入口
│   └── appsettings.json              # 应用配置
│
├── Grassroots.Application/           # 应用层
│   ├── Commands/                     # 命令模式实现
│   │   ├── ICommand.cs               # 命令接口
│   │   └── ICommandHandler.cs        # 命令处理接口
│   ├── Dispatchers/                  # 调度器
│   │   ├── ICommandDispatcher.cs     # 命令调度器接口
│   │   └── IQueryDispatcher.cs       # 查询调度器接口
│   └── Queries/                      # 查询模式实现
│       ├── IQuery.cs                 # 查询接口
│       └── IQueryHandler.cs          # 查询处理接口
│
├── Grassroots.Domain/                # 领域层
│   ├── AggregateRoots/               # 聚合根
│   │   └── AggregateRoot.cs          # 聚合根基类
│   ├── Entities/                     # 实体
│   │   └── Todo.cs                   # 示例待办事项实体
│   ├── Entity/                       # 实体基础
│   │   └── BaseEntity.cs             # 实体基类
│   ├── Events/                       # 领域事件
│   │   └── DomainEvent.cs            # 领域事件基类
│   ├── Repositories/                 # 仓储接口
│   │   └── IRepository.cs            # 通用仓储接口
│   └── ValueObjects/                 # 值对象
│       └── ValueObject.cs            # 值对象基类
│
├── Grassroots.Model/                 # 模型层
│   ├── DTO/                          # 数据传输对象
│   │   └── BaseDto.cs                # 基础数据传输对象
│   └── Mapping/                      # 模型映射
│       └── IMapper.cs                # 映射接口
│
├── Grassroots.Infrastructure/        # 基础设施层
│   ├── Commands/                     # 命令实现
│   │   └── CommandDispatcher.cs      # 命令调度器实现
│   ├── Data/                         # 数据访问
│   │   ├── DbContext/                # 数据上下文
│   │   │   └── ApplicationDbContext.cs # 应用数据上下文
│   │   └── Repositories/             # 仓储实现
│   │       └── Repository.cs         # 通用仓储实现
│   ├── DependencyInjection/          # 依赖注入
│   │   └── ServiceCollectionExtensions.cs # 服务注册扩展
│   ├── Mapping/                      # 映射实现
│   │   └── AutoMapperAdapter.cs      # AutoMapper适配器
│   └── Queries/                      # 查询实现
│       └── QueryDispatcher.cs        # 查询调度器实现
│
└── README.md                         # 项目说明文档
```

## 快速入门

### 添加新实体

1. 在Domain层创建实体类，继承自`BaseEntity`或`AggregateRoot`
2. 在Infrastructure层添加实体配置
3. 在Model层添加相应的DTO
4. 在Application层添加命令和查询处理器
5. 在API层暴露接口

### 注册服务

```csharp
// 在Program.cs中注册服务
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
```

## 运行项目

### 前提条件

- 安装 [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- 安装 [SQL Server](https://www.microsoft.com/sql-server/)（或修改连接字符串使用其他数据库）
- 安装 [Visual Studio 2022](https://visualstudio.microsoft.com/) 或 [Visual Studio Code](https://code.visualstudio.com/)（可选）

### 命令行运行

1. **克隆仓库**
   ```bash
   git clone https://github.com/yourusername/Grassroots.git
   cd Grassroots
   ```

2. **构建项目**
   ```bash
   dotnet build
   ```

3. **运行API项目**
   ```bash
   cd Grassroots.Api
   dotnet run
   ```
   服务将启动并监听在 http://localhost:5143

4. **访问Swagger UI**
   浏览器打开 http://localhost:5143/swagger 查看API文档并测试接口

5. **停止应用程序**
   在命令行中按 `Ctrl+C` 停止应用程序运行

### 使用Visual Studio运行

1. 打开 `Grassroots.sln` 解决方案文件
2. 将 `Grassroots.Api` 设置为启动项目
3. 按 `F5` 启动调试，或 `Ctrl+F5` 启动不调试

### 数据库配置

如需配置数据库连接，请修改 `Grassroots.Api/appsettings.json` 文件：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=GrassrootsDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  ...
}
```

### 数据库迁移

1. **创建迁移**
   ```bash
   cd Grassroots.Infrastructure
   dotnet ef migrations add InitialCreate --startup-project ../Grassroots.Api
   ```

2. **应用迁移**
   ```bash
   dotnet ef database update --startup-project ../Grassroots.Api
   ```

## 使用示例

### 创建控制器

```csharp
[ApiController]
[Route("api/[controller]")]
public class TodosController : ApiBaseController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoDto>>> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllTodosQuery();
        var result = await QueryDispatcher.QueryAsync<GetAllTodosQuery, IEnumerable<TodoDto>>(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateTodoCommand command, CancellationToken cancellationToken)
    {
        var result = await CommandDispatcher.SendAsync<CreateTodoCommand, Guid>(command, cancellationToken);
        return Ok(result);
    }
}
```

## 贡献与反馈

欢迎提交Issue和Pull Request，一起完善Grassroots框架！ 