using Grassroots.Domain.Entities;
using Grassroots.Infrastructure.Data.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        /// <returns>操作结果</returns>
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

                logger.LogInformation("开始初始化数据库...");
                
                // 确保数据库被创建
                await context.Database.EnsureCreatedAsync();
                logger.LogInformation("数据库创建成功，或已存在");
                
                // 如果Todo表为空，则添加示例数据
                if (!await context.Set<Todo>().AnyAsync())
                {
                    logger.LogInformation("正在添加示例待办事项数据...");
                    
                    await context.Set<Todo>().AddRangeAsync(
                        new Todo("完成Grassroots框架", "实现基本的DDD架构和示例API"),
                        new Todo("学习.NET 8新特性", "研究.NET 8中的新功能和改进"),
                        new Todo("准备技术分享", "准备关于DDD的技术分享资料", DateTime.Now.AddDays(7)),
                        new Todo("代码审查", "审查团队成员提交的代码", DateTime.Now.AddDays(2))
                    );
                    
                    await context.SaveChangesAsync();
                    logger.LogInformation("示例数据添加完成");
                }
                else
                {
                    logger.LogInformation("数据库中已存在数据，跳过初始化示例数据");
                }
                
                logger.LogInformation("数据库初始化完成");
            }
            catch (Exception ex)
            {
                using var scope = serviceProvider.CreateScope();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
                logger.LogError(ex, "初始化数据库时发生错误: {ErrorMessage}", ex.Message);
                
                // 如果有内部异常，记录更详细的信息
                if (ex.InnerException != null)
                {
                    logger.LogError(ex.InnerException, "内部异常: {ErrorMessage}", ex.InnerException.Message);
                }
                
                logger.LogError("错误堆栈: {StackTrace}", ex.StackTrace);
                throw;
            }
        }
    }
} 