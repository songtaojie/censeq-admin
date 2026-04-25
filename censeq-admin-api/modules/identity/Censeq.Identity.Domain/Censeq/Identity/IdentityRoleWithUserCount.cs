using System;

namespace Censeq.Identity;

/// <summary>
/// 身份角色With用户数量
/// </summary>
public class IdentityRoleWithUserCount
{
    /// <summary>
    /// 身份角色
    /// </summary>
    public IdentityRole Role { get; set; }

    /// <summary>
    /// long
    /// </summary>
    public long UserCount { get; set; }
    
    public IdentityRoleWithUserCount(IdentityRole role, long userCount)
    {
        Role = role;
        UserCount = userCount;
    }
}
