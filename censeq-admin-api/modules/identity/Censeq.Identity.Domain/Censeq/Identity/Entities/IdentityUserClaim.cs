using System;
using System.Security.Claims;
using JetBrains.Annotations;

namespace Censeq.Identity.Entities;

/// <summary>
/// 用户拥有的声明
/// </summary>
public class IdentityUserClaim : IdentityClaim
{
    /// <summary>
    /// 关联用户的标识
    /// </summary>
    public virtual Guid UserId { get; protected set; }

    protected IdentityUserClaim()
    {

    }

    protected internal IdentityUserClaim(Guid id, Guid userId, [NotNull] Claim claim, Guid? tenantId)
        : base(id, claim, tenantId)
    {
        UserId = userId;
    }

    public IdentityUserClaim(Guid id, Guid userId, [NotNull] string claimType, string claimValue, Guid? tenantId)
        : base(id, claimType, claimValue, tenantId)
    {
        UserId = userId;
    }
}
