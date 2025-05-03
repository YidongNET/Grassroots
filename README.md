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

## 项目结构

- **Grassroots.Api**：Web API项目，处理HTTP请求
- **Grassroots.Application**：应用服务，实现用例和业务流程
- **Grassroots.Domain**：领域层，包含领域模型、领域服务和业务规则
- **Grassroots.Model**：模型定义，包含实体和值对象
- **Grassroots.Infrastructure**：基础设施实现，如数据库访问、日志、消息等

## 如何使用

### 前提条件

- .NET 8 SDK
- Visual Studio 2022或其他兼容IDE
- SQL Server/PostgreSQL/MySQL（根据您的选择）

### 克隆仓库

```bash
git clone https://github.com/your-username/Grassroots.git
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