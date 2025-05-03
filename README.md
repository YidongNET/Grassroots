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

### 服务注册流程

1. `AddInfrastructure` - 注册基础设施服务，包括命令/查询调度器和AutoMapper
2. `AddApplication` - 注册应用层服务，包括命令和查询处理器
3. `AddDatabaseServices` - 注册数据库相关服务，包括DbContext和仓储

## 特性

- 基于.NET 8平台
- 遵循领域驱动设计(DDD)原则
- 采用命令查询职责分离(CQRS)模式
- 支持依赖注入
- 提供通用仓储模式实现
- RESTful API支持
- OpenAPI/Swagger集成
- 强类型配置

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

## 运行说明

### 前提条件

- .NET 8 SDK
- Visual Studio 2022或其他兼容IDE
- SQL Server/PostgreSQL/MySQL（根据您的选择）

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

### 数据库配置

1. 在`Grassroots.Api/appsettings.json`中修改连接字符串和提供程序类型
2. 运行迁移命令：
   ```bash
   cd Grassroots.Infrastructure
   dotnet ef database update --startup-project ../Grassroots.Api
   ```

## 示例

框架附带了一个待办事项(Todo)API示例，展示了完整的CRUD操作：

- `GET /api/todos` - 获取所有待办事项
- `GET /api/todos/{id}` - 根据ID获取待办事项
- `POST /api/todos` - 创建新的待办事项
- `PUT /api/todos/{id}` - 更新现有待办事项
- `DELETE /api/todos/{id}` - 删除待办事项

## 贡献

欢迎提交问题和拉取请求，一起改进Grassroots框架！

## 许可

本项目采用MIT许可证。 