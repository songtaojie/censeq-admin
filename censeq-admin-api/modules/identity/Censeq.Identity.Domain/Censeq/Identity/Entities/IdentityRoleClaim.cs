using System;
using System.Security.Claims;
using JetBrains.Annotations;

namespace Censeq.Identity.Entities;

/// <summary>
/// 表示授予角色内所有用户的声明。
/// </summary>
public class IdentityRoleClaim : IdentityClaim
{
    /// <summary>
    /// 获取或设置与此声明关联的角色的主键。
    /// </summary>
    public virtual Guid RoleId { get; protected set; }

    protected IdentityRoleClaim()
    {

    }

    /// <summary>
    /// 使用指定的角色和声明初始化 <see cref="IdentityRoleClaim"/> 类的新实例。
    /// </summary>
    protected internal IdentityRoleClaim(
        Guid id,
        Guid roleId,
        [NotNull] Claim claim,
        Guid? tenantId)
        : base(
              id,
              claim,
              tenantId)
    {
        RoleId = roleId;
    }

    public IdentityRoleClaim(
        Guid id,
        Guid roleId,
        [NotNull] string claimType,
        string claimValue,
        Guid? tenantId)
        : base(
              id,
              claimType,
              claimValue,
              tenantId)
    {
        RoleId = roleId;
    }
}
