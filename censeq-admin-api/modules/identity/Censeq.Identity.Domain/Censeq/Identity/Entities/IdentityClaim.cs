using System;
using System.Security.Claims;
using JetBrains.Annotations;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Censeq.Identity.Entities;

/// <summary>
/// 身份声明
/// </summary>
public abstract class IdentityClaim : Entity<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; protected set; }

    /// <summary>
    /// 获取或设置此声明的类型。
    /// </summary>
    public virtual string ClaimType { get; protected set; }

    /// <summary>
    /// 获取或设置此声明的值。
    /// </summary>
    public virtual string ClaimValue { get; protected set; }

    protected IdentityClaim()
    {

    }

    /// <summary>
    /// 使用指定的声明初始化 <see cref="IdentityClaim"/> 类的新实例。
    /// </summary>
    protected internal IdentityClaim(Guid id, [NotNull] Claim claim, Guid? tenantId)
        : this(id, claim.Type, claim.Value, tenantId)
    {

    }

    /// <summary>
    /// 使用指定的声明类型和值初始化 <see cref="IdentityClaim"/> 类的新实例。
    /// </summary>
    protected internal IdentityClaim(Guid id, [NotNull] string claimType, string claimValue, Guid? tenantId)
    {
        Check.NotNull(claimType, nameof(claimType));

        Id = id;
        ClaimType = claimType;
        ClaimValue = claimValue;
        TenantId = tenantId;
    }

    /// <summary>
    /// 从此实体创建一个 <see cref="Claim"/> 实例。
    /// </summary>
    /// <returns>表示此实体的 <see cref="Claim"/> 实例。</returns>
    public virtual Claim ToClaim()
    {
        return new Claim(ClaimType, ClaimValue);
    }

    public virtual void SetClaim([NotNull] Claim claim)
    {
        Check.NotNull(claim, nameof(claim));

        ClaimType = claim.Type;
        ClaimValue = claim.Value;
    }
}
