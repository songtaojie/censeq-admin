using System;
using JetBrains.Annotations;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Censeq.Identity.Entities;

/// <summary>
/// 用户认证令牌
/// </summary>
public class IdentityUserToken : Entity, IMultiTenant
{
    public virtual Guid? TenantId { get; protected set; }

    /// <summary>
    /// 令牌所属用户的主键
    /// </summary>
    public virtual Guid UserId { get; protected set; }

    /// <summary>
    /// 令牌的登录提供程序
    /// </summary>
    public virtual string LoginProvider { get; protected set; }

    /// <summary>
    /// 令牌名称
    /// </summary>
    public virtual string Name { get; protected set; }

    /// <summary>
    /// 令牌值
    /// </summary>
    public virtual string Value { get; set; }

    protected IdentityUserToken()
    {

    }

    protected internal IdentityUserToken(
        Guid userId,
        [NotNull] string loginProvider,
        [NotNull] string name,
        string value,
        Guid? tenantId)
    {
        Check.NotNull(loginProvider, nameof(loginProvider));
        Check.NotNull(name, nameof(name));

        UserId = userId;
        LoginProvider = loginProvider;
        Name = name;
        Value = value;
        TenantId = tenantId;
    }

    public override object[] GetKeys()
    {
        return new object[] { UserId, LoginProvider, Name };
    }
}
