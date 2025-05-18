namespace Grassroots.Domain.Interfaces;

/// <summary>
/// ID生成器接口
/// </summary>
public interface IIdGenerator
{
    /// <summary>
    /// 生成新的ID
    /// </summary>
    /// <returns>唯一ID</returns>
    long NextId();
} 