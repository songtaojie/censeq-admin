using System.Security.Claims;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Censeq.Abp.Users;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Guids;

namespace Censeq.Abp.Identity;

/// <summary>
/// �����û�
/// </summary>
public class IdentityUser : FullAuditedAggregateRoot<Guid>, IUser, IHasEntityVersion
{
    /// <summary>
    /// �⻧id
    /// </summary>
    public virtual Guid? TenantId { get; protected set; }

    /// <summary>
    /// ��ȡ�����ô��û����û�����
    /// </summary>
    public virtual string UserName { get; protected internal set; }

    /// <summary>
    ///��ȡ�����ô��û��Ĺ淶���û�����
    /// </summary>
    [DisableAuditing]
    public virtual string NormalizedUserName { get; protected internal set; }

    /// <summary>
    /// ��ȡ�������û������ơ�
    /// </summary>
    public virtual string? Name { get; set; }

    /// <summary>
    /// ��ȡ�������û������ϡ�
    /// </summary>
    public virtual string? Surname { get; set; }

    /// <summary>
    /// ��ȡ�����ô��û��ĵ����ʼ���ַ��
    /// </summary>
    public virtual string Email { get; protected internal set; }

    /// <summary>
    /// ��ȡ�����ô��û��Ĺ淶�������ʼ���ַ��
    /// </summary>
    [DisableAuditing]
    public virtual string NormalizedEmail { get; protected internal set; }

    /// <summary>
    ///��ȡ������һ����־��ָʾ�û��Ƿ���ȷ��������ʼ���ַ��
    /// </summary>
    /// <value>��������ʼ���ַ��ȷ����Ϊ True������Ϊ false.</value>
    public virtual bool EmailConfirmed { get; protected internal set; }

    /// <summary>
    /// ��ȡ�����ô��û�������ļ��κ�ɢ�б�ʾ��ʽ��
    /// </summary>
    [DisableAuditing]
    public virtual string? PasswordHash { get; protected internal set; }

    /// <summary>
    /// ���û�ƾ֤�����ı䣨������ġ���¼ɾ����ʱ������ĵ����ֵ
    /// </summary>
    [DisableAuditing]
    public virtual string SecurityStamp { get; protected internal set; }

    /// <summary>
    /// �Ƿ�Ϊ�ⲿ
    /// </summary>
    public virtual bool IsExternal { get; set; }

    /// <summary>
    /// ��ȡ�������û��ĵ绰���롣
    /// </summary>
    public virtual string? PhoneNumber { get; protected internal set; }

    /// <summary>
    /// ��ȡ������һ����־��ָʾ�û��Ƿ���ȷ����绰��ַ��
    /// </summary>
    /// <value>����绰������ȷ����Ϊ True������Ϊ false.</value>
    public virtual bool PhoneNumberConfirmed { get; protected internal set; }

    /// <summary>
    ///��ȡ������ָʾ�û��Ƿ��ڻ״̬�ı�־��
    /// </summary>
    public virtual bool IsActive { get; protected internal set; }

    /// <summary>
    ///��ȡ������һ����־��ָʾ�Ƿ�Ϊ���û�����˫����������֤��
    /// </summary>
    /// <value>��������� 2fa����Ϊ True������Ϊ false.</value>
    public virtual bool TwoFactorEnabled { get; protected internal set; }

    /// <summary>
    /// ��ȡ�������κ��û������Ľ������ں�ʱ�䣨UTC����
    /// </summary>
    /// <remarks>
    ///��ȥ��ֵ��ζ���û�û�б�������
    /// </remarks>
    public virtual DateTimeOffset? LockoutEnd { get; protected internal set; }

    /// <summary>
    ///��ȡ������һ����־��ָʾ�û��Ƿ���Ա�������
    /// </summary>
    /// <value>����û����ܱ�������Ϊ True������Ϊ false.</value>
    public virtual bool LockoutEnabled { get; protected internal set; }

    /// <summary>
    /// ��ȡ�����õ�ǰ�û���¼ʧ�ܵĴ�����
    /// </summary>
    public virtual int AccessFailedCount { get; protected internal set; }

    /// <summary>
    /// �´ε�¼ʱӦ�������롣
    /// </summary>
    public virtual bool ShouldChangePasswordOnNextLogin { get; protected internal set; }

    /// <summary>
    ///ÿ��ʵ�巢���仯ʱ���汾ֵ�ͻ����ӡ�
    /// </summary>
    public virtual int EntityVersion { get; protected set; }

    /// <summary>
    ///��ȡ�������û��ϴθ��������ʱ�䡣
    /// </summary>
    public virtual DateTimeOffset? LastPasswordChangeTime { get; protected set; }

    /// <summary>
    /// ���û�������ɫ�ĵ������ԡ�
    /// </summary>
    public virtual ICollection<IdentityUserRole> Roles { get; protected set; }

    /// <summary>
    ///���û�ӵ�е������ĵ������ԡ�
    /// </summary>
    public virtual ICollection<IdentityUserClaim> Claims { get; protected set; }

    /// <summary>
    ///���û���¼�ʻ��ĵ������ԡ�
    /// </summary>
    public virtual ICollection<IdentityUserLogin> Logins { get; protected set; }

    /// <summary>
    /// ���û����Ƶĵ������ԡ�
    /// </summary>
    public virtual ICollection<IdentityUserToken> Tokens { get; protected set; }

    /// <summary>
    ///����֯��λ�ĵ������ԡ�
    /// </summary>
    public virtual ICollection<IdentityUserOrganizationUnit> OrganizationUnits { get; protected set; }

    /// <summary>
    /// ���캯��
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userName"></param>
    /// <param name="email"></param>
    /// <param name="tenantId"></param>
    public IdentityUser(Guid id, string userName,string email,Guid? tenantId) : base(id)
    {
        UserName = userName;
        NormalizedUserName = UserName.ToUpperInvariant();
        Email = email;
        NormalizedEmail = Email.ToUpperInvariant();
        ConcurrencyStamp = Guid.NewGuid().ToString("N");
        SecurityStamp = Guid.NewGuid().ToString("N");
        TenantId = tenantId;
        IsActive = true;

        Roles = [];
        Claims = [];
        Logins = [];
        Tokens = [];
        OrganizationUnits = [];
    }

    /// <summary>
    /// ���ӽ�ɫ
    /// </summary>
    /// <param name="roleId"></param>
    public virtual void AddRole(Guid roleId)
    {
        Check.NotNull(roleId, nameof(roleId));
        if (IsInRole(roleId))
        {
            return;
        }
        Roles.Add(new IdentityUserRole(Id, roleId, TenantId));
    }

    /// <summary>
    /// �Ƴ���ɫ
    /// </summary>
    /// <param name="roleId"></param>
    public virtual void RemoveRole(Guid roleId)
    {
        Check.NotNull(roleId, nameof(roleId));
        if (!IsInRole(roleId))
        {
            return;
        }
        Roles.RemoveAll(r => r.RoleId == roleId);
    }

    /// <summary>
    /// �Ƿ��ڽ�ɫ��
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    public virtual bool IsInRole(Guid roleId)
    {
        Check.NotNull(roleId, nameof(roleId));
        return Roles.Any(r => r.RoleId == roleId);
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="guidGenerator"></param>
    /// <param name="claim"></param>
    public virtual void AddClaim([NotNull] IGuidGenerator guidGenerator, [NotNull] Claim claim)
    {
        Check.NotNull(guidGenerator, nameof(guidGenerator));
        Check.NotNull(claim, nameof(claim));
        Claims.Add(new IdentityUserClaim(guidGenerator.Create(), Id, claim, TenantId));
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="guidGenerator"></param>
    /// <param name="claims"></param>
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
    /// ��ȡ����
    /// </summary>
    /// <param name="claim"></param>
    /// <returns></returns>
    public virtual IdentityUserClaim? FindClaim([NotNull] Claim claim)
    {
        Check.NotNull(claim, nameof(claim));
        return Claims.FirstOrDefault(c => c.ClaimType == claim.Type && c.ClaimValue == claim.Value);
    }

    /// <summary>
    /// �滻����
    /// </summary>
    /// <param name="claim"></param>
    /// <param name="newClaim"></param>
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

    /// <summary>
    /// �Ƴ�����
    /// </summary>
    /// <param name="claims"></param>
    public virtual void RemoveClaims([NotNull] IEnumerable<Claim> claims)
    {
        Check.NotNull(claims, nameof(claims));
        foreach (var claim in claims)
        {
            RemoveClaim(claim);
        }
    }

    /// <summary>
    /// �Ƴ�����
    /// </summary>
    /// <param name="claim"></param>
    public virtual void RemoveClaim([NotNull] Claim claim)
    {
        Check.NotNull(claim, nameof(claim));
        Claims.RemoveAll(c => c.ClaimValue == claim.Value && c.ClaimType == claim.Type);
    }

    /// <summary>
    /// ���ӵ�¼��Ϣ
    /// </summary>
    /// <param name="login"></param>
    public virtual void AddLogin([NotNull] UserLoginInfo login)
    {
        Check.NotNull(login, nameof(login));
        Logins.Add(new IdentityUserLogin(Id, login, TenantId));
    }

    /// <summary>
    /// �Ƴ���¼��Ϣ
    /// </summary>
    /// <param name="loginProvider"></param>
    /// <param name="providerKey"></param>
    public virtual void RemoveLogin([NotNull] string loginProvider, [NotNull] string providerKey)
    {
        Check.NotNull(loginProvider, nameof(loginProvider));
        Check.NotNull(providerKey, nameof(providerKey));
        Logins.RemoveAll(userLogin =>
            userLogin.LoginProvider == loginProvider && userLogin.ProviderKey == providerKey);
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="loginProvider"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public virtual IdentityUserToken? FindToken(string loginProvider, string name)
    {
        return Tokens.FirstOrDefault(t => t.LoginProvider == loginProvider && t.Name == name);
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="loginProvider"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
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

    /// <summary>
    /// �Ƴ�����
    /// </summary>
    /// <param name="loginProvider"></param>
    /// <param name="name"></param>
    public virtual void RemoveToken(string loginProvider, string name)
    {
        Tokens.RemoveAll(t => t.LoginProvider == loginProvider && t.Name == name);
    }

    /// <summary>
    /// ������֯��λ
    /// </summary>
    /// <param name="organizationUnitId"></param>
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

    /// <summary>
    /// �Ƴ���֯��Ԫ
    /// </summary>
    /// <param name="organizationUnitId"></param>
    public virtual void RemoveOrganizationUnit(Guid organizationUnitId)
    {
        if (!IsInOrganizationUnit(organizationUnitId))
        {
            return;
        }

        OrganizationUnits.RemoveAll(ou => ou.OrganizationUnitId == organizationUnitId);
    }

    /// <summary>
    /// �Ƿ�����֯��λ
    /// </summary>
    /// <param name="organizationUnitId"></param>
    /// <returns></returns>
    public virtual bool IsInOrganizationUnit(Guid organizationUnitId)
    {
        return OrganizationUnits.Any(ou => ou.OrganizationUnitId == organizationUnitId);
    }

    /// <summary>
    /// ʹ�� <see cref="UserManager{TUser}.ConfirmEmailAsync(TUser, string)"/> ���г�������ʼ�ȷ�ϡ�
    /// ʹ�ô��������ȷ�Ϲ��̲�ֱ������ <see cref="EmailConfirmed"/>��
    /// </summary>
    public virtual IdentityUser SetEmailConfirmed(bool confirmed)
    {
        EmailConfirmed = confirmed;
        return this;
    }

    /// <summary>
    /// ���õ绰����ȷ��
    /// </summary>
    /// <param name="confirmed"></param>
    public virtual IdentityUser SetPhoneNumberConfirmed(bool confirmed)
    {
        PhoneNumberConfirmed = confirmed;
        return this;
    }

    /// <summary>
    ///ͨ��ʹ�� <see cref="UserManager{TUser}.ChangePhoneNumberAsync"/> ��Ӧ�ó�������и��ĵ绰���롣�˷�����ֱ��Ϊ������ȷ����Ϣ��
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <param name="confirmed"></param>
    /// <exception cref="NotImplementedException"></exception>
    public IdentityUser SetPhoneNumber(string? phoneNumber, bool confirmed)
    {
        PhoneNumber = phoneNumber;
        PhoneNumberConfirmed = !phoneNumber.IsNullOrWhiteSpace() && confirmed;
        return this;
    }

    /// <summary>
    /// ����״̬
    /// </summary>
    /// <param name="isActive"></param>
    public virtual IdentityUser SetIsActive(bool isActive)
    {
        IsActive = isActive;
        return this;
    }

    /// <summary>
    /// �����´ε�¼ʱӦ��������
    /// </summary>
    /// <param name="shouldChangePasswordOnNextLogin"></param>
    public virtual IdentityUser SetShouldChangePasswordOnNextLogin(bool shouldChangePasswordOnNextLogin)
    {
        ShouldChangePasswordOnNextLogin = shouldChangePasswordOnNextLogin;
        return this;
    }

    /// <summary>
    /// �����ϴ��������ʱ��
    /// </summary>
    /// <param name="lastPasswordChangeTime"></param>
    public virtual IdentityUser SetLastPasswordChangeTime(DateTimeOffset? lastPasswordChangeTime)
    {
        LastPasswordChangeTime = lastPasswordChangeTime;
        return this;
    }

    /// <summary>
    /// ���������ϣ
    /// </summary>
    /// <param name="passwordHash"></param>
    public virtual IdentityUser SetPasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash;
        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"{base.ToString()}, UserName = {UserName}";
    }
}
