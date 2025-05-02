using System.Threading;
using System.Threading.Tasks;
using Grassroots.Application.Commands;

namespace Grassroots.Application.Dispatchers
{
    /// <summary>
    /// 命令调度器接口
    /// </summary>
    public interface ICommandDispatcher
    {
        /// <summary>
        /// 发送命令
        /// </summary>
        /// <typeparam name="TCommand">命令类型</typeparam>
        /// <param name="command">命令</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>操作结果</returns>
        Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) 
            where TCommand : ICommand;

        /// <summary>
        /// 发送带返回值的命令
        /// </summary>
        /// <typeparam name="TCommand">命令类型</typeparam>
        /// <typeparam name="TResult">返回值类型</typeparam>
        /// <param name="command">命令</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>操作结果</returns>
        Task<TResult> SendAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default) 
            where TCommand : ICommand<TResult>;
    }
} 