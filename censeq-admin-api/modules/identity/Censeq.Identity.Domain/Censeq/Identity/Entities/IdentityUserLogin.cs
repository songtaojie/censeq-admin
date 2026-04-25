using System;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Censeq.Identity.Entities;

/// <summary>
/// 用户的登录及其关联提供程序
/// </summary>
public class IdentityUserLogin : Entity, IMultiTenant
{
    public virtual Guid? TenantId { get; protected set; }

    /// <summary>
    /// 关联用户的主键
    /// </summary>
    public virtual Guid UserId { get; protected set; }

    /// <summary>
    /// 登录提供程序（例如 facebook、google）
    /// </summary>
    public virtual string LoginProvider { get; protected set; }

    /// <summary>
    /// 登录的唯一提供程序标识符
    /// </summary>
    public virtual string ProviderKey { get; protected set; }

    /// <summary>
    /// 登录在 UI 中显示的友好名称
    /// </summary>
    public virtual string ProviderDisplayName { get; protected set; }

    protected IdentityUserLogin()
    {

    }

    protected internal IdentityUserLogin(
        Guid userId,
        [NotNull] string loginProvider,
        [NotNull] string providerKey,
        string providerDisplayName,
        Guid? tenantId)
    {
        Check.NotNull(loginProvider, nameof(loginProvider));
        Check.NotNull(providerKey, nameof(providerKey));

        UserId = userId;
        LoginProvider = loginProvider;
        ProviderKey = providerKey;
        ProviderDisplayName = providerDisplayName;
        TenantId = tenantId;
    }

    protected internal IdentityUserLogin(
        Guid userId,
        [NotNull] UserLoginInfo login,
        Guid? tenantId)
        : this(
              userId,
              login.LoginProvider,
              login.ProviderKey,
              login.ProviderDisplayName!,
              tenantId)
    {
    }

    /// <summary>
    /// 转换为用户登录信息
    /// </summary>
    public virtual UserLoginInfo ToUserLoginInfo()
    {
        return new UserLoginInfo(LoginProvider, ProviderKey, ProviderDisplayName);
    }

    public override object[] GetKeys()
    {
        return new object[] { UserId, LoginProvider };
    }
}
