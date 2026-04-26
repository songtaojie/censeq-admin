using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Guids;
using Volo.Abp.Users;

namespace Censeq.Identity.Entities;

/// <summary>
/// 身份用户
/// </summary>
public class IdentityUser : FullAuditedAggregateRoot<Guid>, IUser, IHasEntityVersion
{
    public virtual Guid? TenantId { get; protected set; }

    /// <summary>
    /// 获取或设置此用户的用户名。
    /// </summary>
    public virtual string UserName { get; protected internal set; }

    /// <summary>
    /// 获取或设置此用户的标准化用户名。
    /// </summary>
    [DisableAuditing]
    public virtual string NormalizedUserName { get; protected internal set; }

    /// <summary>
    /// 获取或设置用户的名称。
    /// </summary>
    [CanBeNull]
    public virtual string? Name { get; set; }

    /// <summary>
    /// 获取或设置用户的姓氏。
    /// </summary>
    [CanBeNull]
    public virtual string? Surname { get; set; }

    /// <summary>
    /// 获取或设置此用户的电子邮箱地址。
    /// </summary>
    public virtual string Email { get; protected internal set; }

    /// <summary>
    /// 获取或设置此用户的标准化电子邮箱地址。
    /// </summary>
    [DisableAuditing]
    public virtual string NormalizedEmail { get; protected internal set; }

    /// <summary>
    /// 获取或设置一个标志，指示用户是否已确认其电子邮箱地址。
    /// </summary>
    /// <value>如果电子邮箱地址已确认则为 true，否则为 false。</value>
    public virtual bool EmailConfirmed { get; protected internal set; }

    /// <summary>
    /// 获取或设置此用户密码的加盐哈希表示。
    /// </summary>
    [DisableAuditing]
    public virtual string PasswordHash { get; protected internal set; }

    /// <summary>
    /// 一个随机值，每当用户凭据更改时（密码更改、登录移除）必须更改。
    /// </summary>
    [DisableAuditing]
    public virtual string SecurityStamp { get; protected internal set; }

    public virtual bool IsExternal { get; set; }

    /// <summary>
    /// 获取或设置用户的电话号码。
    /// </summary>
    [CanBeNull]
    public virtual string PhoneNumber { get; protected internal set; }

    /// <summary>
    /// 获取或设置一个标志，指示用户是否已确认其电话号码。
    /// </summary>
    /// <value>如果电话号码已确认则为 true，否则为 false。</value>
    public virtual bool PhoneNumberConfirmed { get; protected internal set; }

    /// <summary>
    /// 获取或设置一个标志，指示用户是否处于活动状态。
    /// </summary>
    public virtual bool IsActive { get; protected internal set; }

    /// <summary>
    /// 获取或设置一个标志，指示是否为此用户启用了双重身份验证。
    /// </summary>
    /// <value>如果启用了双重身份验证则为 true，否则为 false。</value>
    public virtual bool TwoFactorEnabled { get; protected internal set; }

    /// <summary>
    /// 获取或设置用户锁定结束的UTC日期时间
    /// </summary>
    /// <remarks>
    /// 过去的时间值表示用户未被锁定
    /// </remarks>
    public virtual DateTimeOffset? LockoutEnd { get; protected internal set; }

    /// <summary>
    /// 获取或设置一个标志，指示是否可以锁定用户。
    /// </summary>
    /// <value>如果可以锁定用户则为 true，否则为 false。</value>
    public virtual bool LockoutEnabled { get; protected internal set; }

    /// <summary>
    /// 获取或设置当前用户的失败登录尝试次数。
    /// </summary>
    public virtual int AccessFailedCount { get; protected internal set; }

    /// <summary>
    /// 下次登录时是否应更改密码。
    /// </summary>
    public virtual bool ShouldChangePasswordOnNextLogin { get; protected internal set; }

    /// <summary>
    /// 实体发生更改时递增的版本值。
    /// </summary>
    public virtual int EntityVersion { get; protected set; }

    /// <summary>
    /// 获取或设置用户上次更改密码的时间。
    /// </summary>
    public virtual DateTimeOffset? LastPasswordChangeTime { get; protected set; }

    //TODO: Can we make collections readonly collection, which will provide encapsulation. But... can work for all ORMs?

    /// <summary>
    /// 此用户所属角色的导航属性。
    /// </summary>
    public virtual ICollection<IdentityUserRole> Roles { get; protected set; }

    /// <summary>
    /// 此用户拥有的声明的导航属性。
    /// </summary>
    public virtual ICollection<IdentityUserClaim> Claims { get; protected set; }

    /// <summary>
    /// 此用户登录账户的导航属性。
    /// </summary>
    public virtual ICollection<IdentityUserLogin> Logins { get; protected set; }

    /// <summary>
    /// 此用户令牌的导航属性。
    /// </summary>
    public virtual ICollection<IdentityUserToken> Tokens { get; protected set; }

    /// <summary>
    /// 此用户所属组织单元的导航属性。
    /// </summary>
    public virtual ICollection<IdentityUserOrganizationUnit> OrganizationUnits { get; protected set; }

    protected IdentityUser()
    {
    }

    public IdentityUser(
        Guid id,
        [NotNull] string userName,
        [NotNull] string email,
        Guid? tenantId = null)
        : base(id)
    {
        Check.NotNull(userName, nameof(userName));
        Check.NotNull(email, nameof(email));

        TenantId = tenantId;
        UserName = userName;
        NormalizedUserName = userName.ToUpperInvariant();
        Email = email;
        NormalizedEmail = email.ToUpperInvariant();
        ConcurrencyStamp = Guid.NewGuid().ToString("N");
        SecurityStamp = Guid.NewGuid().ToString();
        IsActive = true;

        Roles = new Collection<IdentityUserRole>();
        Claims = new Collection<IdentityUserClaim>();
        Logins = new Collection<IdentityUserLogin>();
        Tokens = new Collection<IdentityUserToken>();
        OrganizationUnits = new Collection<IdentityUserOrganizationUnit>();
    }

    public virtual void AddRole(Guid roleId)
    {
        Check.NotNull(roleId, nameof(roleId));

        if (IsInRole(roleId))
        {
            return;
        }

        Roles.Add(new IdentityUserRole(Id, roleId, TenantId));
    }

    public virtual void RemoveRole(Guid roleId)
    {
        Check.NotNull(roleId, nameof(roleId));

        if (!IsInRole(roleId))
        {
            return;
        }

        Roles.RemoveAll(r => r.RoleId == roleId);
    }

    public virtual bool IsInRole(Guid roleId)
    {
        Check.NotNull(roleId, nameof(roleId));

        return Roles.Any(r => r.RoleId == roleId);
    }

    public virtual void AddClaim([NotNull] IGuidGenerator guidGenerator, [NotNull] Claim claim)
    {
        Check.NotNull(guidGenerator, nameof(guidGenerator));
        Check.NotNull(claim, nameof(claim));

        Claims.Add(new IdentityUserClaim(guidGenerator.Create(), Id, claim, TenantId));
    }

    public virtual void AddClaims([NotNull] IGuidGenerator guidGenerator, [NotNull] IEnumerable<Claim> claims)
    {
        Check.NotNull(guidGenerator, nameof(guidGenerator));
        Check.NotNull(claims, nameof(claims));

        foreach (var claim in claims)
        {
            AddClaim(guidGenerator, claim);
        }
    }

    /// <summary>
    /// 身份用户声明
    /// </summary>
    public virtual IdentityUserClaim FindClaim([NotNull] Claim claim)
    {
        Check.NotNull(claim, nameof(claim));

        return Claims.FirstOrDefault(c => c.ClaimType == claim.Type && c.ClaimValue == claim.Value)!;
    }

    public virtual void ReplaceClaim([NotNull] Claim claim, [NotNull] Claim newClaim)
    {
        Check.NotNull(claim, nameof(claim));
        Check.NotNull(newClaim, nameof(newClaim));

        var userClaims = Claims.Where(uc => uc.ClaimValue == claim.Value && uc.ClaimType == claim.Type);
        foreach (var userClaim in userClaims)
        {
            userClaim.SetClaim(newClaim);
        }
    }

    public virtual void RemoveClaims([NotNull] IEnumerable<Claim> claims)
    {
        Check.NotNull(claims, nameof(claims));

        foreach (var claim in claims)
        {
            RemoveClaim(claim);
        }
    }

    public virtual void RemoveClaim([NotNull] Claim claim)
    {
        Check.NotNull(claim, nameof(claim));

        Claims.RemoveAll(c => c.ClaimValue == claim.Value && c.ClaimType == claim.Type);
    }

    public virtual void AddLogin([NotNull] UserLoginInfo login)
    {
        Check.NotNull(login, nameof(login));

        Logins.Add(new IdentityUserLogin(Id, login, TenantId));
    }

    public virtual void RemoveLogin([NotNull] string loginProvider, [NotNull] string providerKey)
    {
        Check.NotNull(loginProvider, nameof(loginProvider));
        Check.NotNull(providerKey, nameof(providerKey));

        Logins.RemoveAll(userLogin =>
            userLogin.LoginProvider == loginProvider && userLogin.ProviderKey == providerKey);
    }

    [CanBeNull]
    /// <summary>
    /// 身份用户令牌
    /// </summary>
    public virtual IdentityUserToken FindToken(string loginProvider, string name)
    {
        return Tokens.FirstOrDefault(t => t.LoginProvider == loginProvider && t.Name == name)!;
    }

    public virtual void SetToken(string loginProvider, string name, string value)
    {
        var token = FindToken(loginProvider, name);
        if (token == null)
        {
            Tokens.Add(new IdentityUserToken(Id, loginProvider, name, value, TenantId));
        }
        else
        {
            token.Value = value;
        }
    }

    public virtual void RemoveToken(string loginProvider, string name)
    {
        Tokens.RemoveAll(t => t.LoginProvider == loginProvider && t.Name == name);
    }

    public virtual void AddOrganizationUnit(Guid organizationUnitId)
    {
        if (IsInOrganizationUnit(organizationUnitId))
        {
            return;
        }

        OrganizationUnits.Add(
            new IdentityUserOrganizationUnit(
                Id,
                organizationUnitId,
                TenantId
            )
        );
    }

    public virtual void RemoveOrganizationUnit(Guid organizationUnitId)
    {
        if (!IsInOrganizationUnit(organizationUnitId))
        {
            return;
        }

        OrganizationUnits.RemoveAll(
            ou => ou.OrganizationUnitId == organizationUnitId
        );
    }

    public virtual bool IsInOrganizationUnit(Guid organizationUnitId)
    {
        return OrganizationUnits.Any(
            ou => ou.OrganizationUnitId == organizationUnitId
        );
    }

    /// <summary>
    /// 常规的邮箱确认请使用 <see cref="IdentityUserManager.ConfirmEmailAsync"/>。
    /// 使用此方法将跳过确认过程并直接设置 <see cref="EmailConfirmed"/>。
    /// </summary>
    public virtual void SetEmailConfirmed(bool confirmed)
    {
        EmailConfirmed = confirmed;
    }

    public virtual void SetPhoneNumberConfirmed(bool confirmed)
    {
        PhoneNumberConfirmed = confirmed;
    }

    /// <summary>
    /// 通常请在应用程序代码中使用 <see cref="IdentityUserManager.ChangePhoneNumberAsync"/> 来更改电话号码。
    /// 此方法用于直接设置电话号码及其确认状态。
    /// </summary>
    /// <param name="phoneNumber">电话号码。</param>
    /// <param name="confirmed">是否已确认。</param>
    public void SetPhoneNumber(string phoneNumber, bool confirmed)
    {
        PhoneNumber = phoneNumber;
        PhoneNumberConfirmed = !phoneNumber.IsNullOrWhiteSpace() && confirmed;
    }

    public virtual void SetIsActive(bool isActive)
    {
        IsActive = isActive;
    }

    public virtual void SetShouldChangePasswordOnNextLogin(bool shouldChangePasswordOnNextLogin)
    {
        ShouldChangePasswordOnNextLogin = shouldChangePasswordOnNextLogin;
    }

    public virtual void SetLastPasswordChangeTime(DateTimeOffset? lastPasswordChangeTime)
    {
        LastPasswordChangeTime = lastPasswordChangeTime;
    }

    public override string ToString()
    {
        return $"{base.ToString()}, UserName = {UserName}";
    }
}
