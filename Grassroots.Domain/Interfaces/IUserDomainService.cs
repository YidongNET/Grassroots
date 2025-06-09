using Grassroots.Domain.Entities;
using Grassroots.Domain.ValueObjects;

namespace Grassroots.Domain.Interfaces;

/// <summary>
/// 用户领域服务接口
/// </summary>
public interface IUserDomainService
{
    /// <summary>
    /// 验证用户密码是否符合规则
    /// </summary>
    bool ValidatePassword(string password);

    /// <summary>
    /// 验证用户名是否符合规则
    /// </summary>
    bool ValidateUsername(string username);

    /// <summary>
    /// 检查用户是否可以更改角色
    /// </summary>
    bool CanChangeRole(User user, Role newRole);

    /// <summary>
    /// 验证用户状态变更是否合法
    /// </summary>
    bool ValidateStatusChange(User user, UserStatus newStatus);
} 