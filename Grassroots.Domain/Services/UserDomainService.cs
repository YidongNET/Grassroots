using System.Text.RegularExpressions;
using Grassroots.Domain.Entities;
using Grassroots.Domain.Exceptions;
using Grassroots.Domain.Interfaces;
using Grassroots.Domain.ValueObjects;

namespace Grassroots.Domain.Services;

/// <summary>
/// 用户领域服务实现
/// </summary>
public class UserDomainService : IUserDomainService
{
    private readonly IIdGenerator _idGenerator;

    public UserDomainService(IIdGenerator idGenerator)
    {
        _idGenerator = idGenerator;
    }

    public User CreateUser(string username, string passwordHash)
    {
        if (!ValidateUsername(username))
            throw new DomainException("Invalid username format");

        var id = _idGenerator.NextId();
        return new User(id, username, passwordHash);
    }

    public bool ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        // 密码规则：至少8位，包含大小写字母和数字
        var hasNumber = new Regex(@"[0-9]+");
        var hasUpperChar = new Regex(@"[A-Z]+");
        var hasLowerChar = new Regex(@"[a-z]+");
        var hasMinimum8Chars = new Regex(@".{8,}");

        return hasNumber.IsMatch(password) &&
               hasUpperChar.IsMatch(password) &&
               hasLowerChar.IsMatch(password) &&
               hasMinimum8Chars.IsMatch(password);
    }

    public bool ValidateUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return false;

        // 用户名规则：3-20位字母数字下划线
        var isValid = new Regex(@"^[a-zA-Z0-9_]{3,20}$");
        return isValid.IsMatch(username);
    }

    public bool CanChangeRole(User user, Role newRole)
    {
        // 已删除用户不能更改角色
        if (user.Status == UserStatus.Deleted)
            return false;

        // 不能降级超级管理员
        if (user.Role == Role.SuperAdmin && newRole != Role.SuperAdmin)
            return false;

        // 普通用户不能直接升级为超级管理员
        if (user.Role == Role.User && newRole == Role.SuperAdmin)
            return false;

        return true;
    }

    public bool ValidateStatusChange(User user, UserStatus newStatus)
    {
        // 已删除用户状态不能更改
        if (user.Status == UserStatus.Deleted)
            return false;

        // 超级管理员不能被停用或删除
        if (user.Role == Role.SuperAdmin &&
            (newStatus == UserStatus.Suspended || newStatus == UserStatus.Deleted))
            return false;

        // 不能从删除状态恢复
        if (user.Status == UserStatus.Deleted && newStatus != UserStatus.Deleted)
            return false;

        return true;
    }
} 