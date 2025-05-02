namespace Grassroots.Application.Commands
{
    /// <summary>
    /// 命令接口
    /// </summary>
    public interface ICommand
    {
    }

    /// <summary>
    /// 带返回值的命令接口
    /// </summary>
    /// <typeparam name="TResult">返回值类型</typeparam>
    public interface ICommand<out TResult>
    {
    }
} 