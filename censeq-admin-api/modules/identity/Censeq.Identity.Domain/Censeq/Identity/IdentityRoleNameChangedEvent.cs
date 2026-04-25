using System;
using Censeq.Identity.Entities;

namespace Censeq.Identity;

[Obsolete("Use the distributed event (IdentityRoleNameChangedEto) instead.")]
/// <summary>
/// 身份角色名称Changed事件
/// </summary>
public class IdentityRoleNameChangedEvent
{
    /// <summary>
    /// 身份角色
    /// </summary>
    public IdentityRole IdentityRole { get; set; }
    public string OldName { get; set; }
}
