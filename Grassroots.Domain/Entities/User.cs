using Grassroots.Domain.Common;
using Grassroots.Domain.ValueObjects;

namespace Grassroots.Domain.Entities;

/// <summary>
/// 用户实体
/// </summary>
public class User : AggregateRoot<long>
{
    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; private set; }

    /// <summary>
    /// 密码哈希
    /// </summary>
    public string PasswordHash { get; private set; }

    /// <summary>
    /// 角色
    /// </summary>
    public Role Role { get; private set; }

    /// <summary>
    /// 用户状态
    /// </summary>
    public UserStatus Status { get; private set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// 最后登录时间
    /// </summary>
    public DateTime? LastLoginAt { get; private set; }

    protected User() : base(0) 
    {
        Username = string.Empty;
        PasswordHash = string.Empty;
    }

    public User(long id, string username, string passwordHash) : base(id)
    {
        Username = username;
        PasswordHash = passwordHash;
        Role = Role.User;
        Status = UserStatus.Inactive;
        CreatedAt = DateTime.UtcNow;
    }

    public void ChangeRole(Role newRole)
    {
        Role = newRole;
    }

    public void ChangeStatus(UserStatus newStatus)
    {
        Status = newStatus;
    }

    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    public void UpdatePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
    }
} 