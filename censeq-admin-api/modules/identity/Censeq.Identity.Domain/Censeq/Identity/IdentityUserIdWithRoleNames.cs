using System;
using System.Collections.Generic;

namespace Censeq.Identity;

/// <summary>
/// 身份用户标识With角色名称
/// </summary>
public class IdentityUserIdWithRoleNames
{
    public Guid Id { get; set; }

    /// <summary>
    /// string[]
    /// </summary>
    public string[] RoleNames { get; set; }
}