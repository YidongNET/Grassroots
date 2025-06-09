namespace Grassroots.Domain.Interfaces;

/// <summary>
/// ID生成器接口
/// </summary>
public interface IIdGenerator
{
    /// <summary>
    /// 生成新的ID
    /// </summary>
    /// <returns>雪花算法生成的ID</returns>
    long NextId();
} 