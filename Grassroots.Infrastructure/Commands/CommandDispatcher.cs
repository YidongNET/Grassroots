using System;
using System.Threading;
using System.Threading.Tasks;
using Grassroots.Application.Commands;
using Grassroots.Application.Dispatchers;
using Microsoft.Extensions.DependencyInjection;

namespace Grassroots.Infrastructure.Commands
{
    /// <summary>
    /// 命令分发器实现
    /// </summary>
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="serviceProvider">服务提供者</param>
        public CommandDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 发送命令
        /// </summary>
        /// <typeparam name="TCommand">命令类型</typeparam>
        /// <param name="command">命令</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>操作结果</returns>
        public async Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) 
            where TCommand : ICommand
        {
            var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();
            await handler.HandleAsync(command, cancellationToken);
        }

        /// <summary>
        /// 发送带返回值的命令
        /// </summary>
        /// <typeparam name="TCommand">命令类型</typeparam>
        /// <typeparam name="TResult">返回值类型</typeparam>
        /// <param name="command">命令</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>操作结果</returns>
        public async Task<TResult> SendAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default) 
            where TCommand : ICommand<TResult>
        {
            var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand, TResult>>();
            return await handler.HandleAsync(command, cancellationToken);
        }
    }
} 