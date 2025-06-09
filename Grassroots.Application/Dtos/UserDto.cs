using Grassroots.Domain.ValueObjects;

namespace Grassroots.Application.Dtos;

/// <summary>
/// 用户数据传输对象
/// </summary>
public class UserDto
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 角色
    /// </summary>
    public Role Role { get; set; }

    /// <summary>
    /// 用户状态
    /// </summary>
    public UserStatus Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 最后登录时间
    /// </summary>
    public DateTime? LastLoginAt { get; set; }
} 