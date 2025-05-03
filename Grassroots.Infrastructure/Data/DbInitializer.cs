using Grassroots.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Grassroots.Infrastructure.Data
{
    /// <summary>
    /// 数据库初始化器
    /// </summary>
    public static class DbInitializer
    {
        /// <summary>
        /// 初始化数据库
        /// </summary>
        /// <param name="serviceProvider">服务提供者</param>
        /// <returns>任务</returns>
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await dbContext.Database.MigrateAsync();

            // 检查是否有任何待办事项，没有则添加示例数据
            if (!await dbContext.Todos.AnyAsync())
            {
                var todos = new[]
                {
                    new Todo
                    {
                        Title = "完成Grassroots框架设计",
                        Description = "实现基本功能和架构",
                        Priority = 3,
                        DueDate = DateTime.Now.AddDays(7)
                    },
                    new Todo
                    {
                        Title = "编写单元测试",
                        Description = "为核心功能编写单元测试",
                        Priority = 2,
                        DueDate = DateTime.Now.AddDays(14)
                    },
                    new Todo
                    {
                        Title = "编写文档",
                        Description = "为框架编写详细文档",
                        Priority = 1,
                        DueDate = DateTime.Now.AddDays(21)
                    }
                };

                await dbContext.Todos.AddRangeAsync(todos);
                await dbContext.SaveChangesAsync();
            }
        }
    }
} 