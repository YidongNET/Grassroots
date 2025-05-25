using Grassroots.Domain.Events;

namespace Grassroots.Domain.Entities;

/// <summary>
/// 用户实体
/// </summary>
public class User : Entity<long>
{
    /// <summary>
    /// 用户名
    /// </summary>
    public required string UserName { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 