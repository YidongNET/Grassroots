using System.Threading;
using System.Threading.Tasks;

namespace Grassroots.Application.Commands
{
    /// <summary>
    /// 命令处理接口
    /// </summary>
    /// <typeparam name="TCommand">命令类型</typeparam>
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        /// <summary>
        /// 处理命令
        /// </summary>
        /// <param name="command">命令</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>操作结果</returns>
        Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// 带返回值的命令处理接口
    /// </summary>
    /// <typeparam name="TCommand">命令类型</typeparam>
    /// <typeparam name="TResult">返回值类型</typeparam>
    public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand<TResult>
    {
        /// <summary>
        /// 处理命令
        /// </summary>
        /// <param name="command">命令</param>
        /// <param name="cancellationToken">取消标记</param>
        /// <returns>操作结果</returns>
        Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
    }
} 