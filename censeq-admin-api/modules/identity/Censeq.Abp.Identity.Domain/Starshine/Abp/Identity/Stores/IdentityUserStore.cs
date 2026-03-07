using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Security.Claims;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;

namespace Censeq.Abp.Identity;

/// <summary>
/// ��ʾָ���û��ͽ�ɫ���͵ĳ־��Դ洢����ʵ����
/// </summary>
public class IdentityUserStore :
    IUserLoginStore<IdentityUser>,
    IUserRoleStore<IdentityUser>,
    IUserClaimStore<IdentityUser>,
    IUserPasswordStore<IdentityUser>,
    IUserSecurityStampStore<IdentityUser>,
    IUserEmailStore<IdentityUser>,
    IUserLockoutStore<IdentityUser>,
    IUserPhoneNumberStore<IdentityUser>,
    IUserTwoFactorStore<IdentityUser>,
    IUserAuthenticationTokenStore<IdentityUser>,
    IUserAuthenticatorKeyStore<IdentityUser>,
    IUserTwoFactorRecoveryCodeStore<IdentityUser>,
    ITransientDependency
{
    private const string InternalLoginProvider = "[AspNetUserStore]";
    private const string AuthenticatorKeyTokenName = "AuthenticatorKey";
    private const string RecoveryCodeTokenName = "RecoveryCodes";

    /// <summary>
    /// ��ȡ�����õ�ǰ�����������κδ���� <see cref="IdentityErrorDescriber"/>��
    /// </summary>
    public IdentityErrorDescriber ErrorDescriber { get; set; }

    /// <summary>
    /// ��ȡ������һ����־��ָʾ�ڵ��� CreateAsync��UpdateAsync �� DeleteAsync ���Ƿ�Ӧ�������ġ�
    /// </summary>
    /// <value>
    /// ���Ӧ�Զ��������ģ���Ϊ True������Ϊ false��
    /// </value>
    public bool AutoSaveChanges { get; set; } = true;
    /// <summary>
    /// ��ɫ�洢��
    /// </summary>
    protected IIdentityRoleRepository RoleRepository { get; }
    /// <summary>
    /// Guid������
    /// </summary>
    protected IGuidGenerator GuidGenerator { get; }
    /// <summary>
    /// ��־��¼
    /// </summary>
    protected ILogger<IdentityRoleStore> Logger { get; }
    /// <summary>
    /// ���ҹ淶��
    /// </summary>
    protected ILookupNormalizer LookupNormalizer { get; }
    /// <summary>
    /// �û��洢��
    /// </summary>
    protected IIdentityUserRepository UserRepository { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userRepository"></param>
    /// <param name="roleRepository"></param>
    /// <param name="guidGenerator"></param>
    /// <param name="logger"></param>
    /// <param name="lookupNormalizer"></param>
    /// <param name="describer"></param>
    public IdentityUserStore(
        IIdentityUserRepository userRepository,
        IIdentityRoleRepository roleRepository,
        IGuidGenerator guidGenerator,
        ILogger<IdentityRoleStore> logger,
        ILookupNormalizer lookupNormalizer,
        IdentityErrorDescriber? describer = null)
    {
        UserRepository = userRepository;
        RoleRepository = roleRepository;
        GuidGenerator = guidGenerator;
        Logger = logger;
        LookupNormalizer = lookupNormalizer;

        ErrorDescriber = describer ?? new IdentityErrorDescriber();
    }

    /// <summary>
    /// ��ȡָ�� <paramref name="user"/> ���û���ʶ����
    /// </summary>
    /// <param name="user">Ӧ�������ʶ�����û���</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>The <see cref="Task"/>��ʾ�첽����������ָ�� <paramref name="user"/> �ı�ʶ����</returns>
    public virtual Task<string> GetUserIdAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        return Task.FromResult(user.Id.ToString());
    }

    /// <summary>
    /// ��ȡָ�� <paramref name="user"/> ���û�����
    /// </summary>
    /// <param name="user">Ӧ�������������û���</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>The <see cref="Task"/> �����첽����������ָ�� <paramref name="user"/> �����ơ�</returns>
    public virtual Task<string?> GetUserNameAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        return Task.FromResult<string?>(user.UserName);
    }

    /// <summary>
    /// Ϊָ���� <paramref name="user"/> ���ø����� <paramref name="userName" />��
    /// </summary>
    /// <param name="user">Ӧ���������Ƶ��û���</param>
    /// <param name="userName">The user name to set.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>�����첽������ <see cref="Task"/>��</returns>
    public virtual Task SetUserNameAsync([NotNull] IdentityUser user, string? userName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        Check.NotNull(userName, nameof(userName));
        user.UserName = userName;
        return Task.CompletedTask;
    }

    /// <summary>
    /// ��ȡָ�� <paramref name="user"/> �Ĺ淶���û�����
    /// </summary>
    /// <param name="user">Ӧ������淶�����Ƶ��û���</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>�����첽������ <see cref="Task"/>������ָ�� <paramref name="user"/> �Ĺ淶���û�����</returns>
    public virtual Task<string?> GetNormalizedUserNameAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        return Task.FromResult<string?>(user.NormalizedUserName);
    }

    /// <summary>
    /// Ϊָ���� <paramref name="user"/> ���ø����Ĺ淶�����ơ�
    /// </summary>
    /// <param name="user">Ӧ���������Ƶ��û���</param>
    /// <param name="normalizedName">Ҫ���õĹ淶�����ơ�</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>�����첽������ <see cref="Task"/>��</returns>
    public virtual Task SetNormalizedUserNameAsync([NotNull] IdentityUser user, string? normalizedName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        Check.NotNull(normalizedName, nameof(normalizedName));
        user.NormalizedUserName = normalizedName;
        return Task.CompletedTask;
    }

    /// <summary>
    /// ���û��洢�д���ָ���� <paramref name="user"/>��
    /// </summary>
    /// <param name="user">Ҫ�������û���</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>The <see cref="Task"/> ��ʾ�첽��������������������<see cref="IdentityResult"/>��</returns>
    public virtual async Task<IdentityResult> CreateAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        await UserRepository.InsertAsync(user, AutoSaveChanges, cancellationToken);
        return IdentityResult.Success;
    }

    /// <summary>
    /// �����û��洢��ָ���� <paramref name="user"/>��
    /// </summary>
    /// <param name="user">Ҫ���µ��û���</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>�����첽������<see cref="Task"/>���������²�����<see cref="IdentityResult"/>��</returns>
    public virtual async Task<IdentityResult> UpdateAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));

        try
        {
            await UserRepository.UpdateAsync(user, AutoSaveChanges, cancellationToken);
        }
        catch (AbpDbConcurrencyException ex)
        {
            Logger.LogError(ex, ex.Message); //Trigger some AbpHandledException event
            return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
        }

        return IdentityResult.Success;
    }

    /// <summary>
    /// ���û��洢��ɾ��ָ���� <paramref name="user"/>��
    /// </summary>
    /// <param name="user">The user to delete.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>�����첽������<see cref="Task"/>���������²�����<see cref="IdentityResult"/>��</returns>
    public virtual async Task<IdentityResult> DeleteAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        try
        {
            await UserRepository.DeleteAsync(user, AutoSaveChanges, cancellationToken);
        }
        catch (AbpDbConcurrencyException ex)
        {
            Logger.LogError(ex, ex.Message); //Trigger some AbpHandledException event
            return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
        }

        return IdentityResult.Success;
    }

    /// <summary>
    /// ���Ҳ����ؾ���ָ�� <paramref name="userId"/> ���û�������У���
    /// </summary>
    /// <param name="userId">Ҫ�������û� ID��</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>
    /// ��ʾ�첽������<see cref="Task"/>��������ָ��<paramref name="userId"/>ƥ����û���������ڣ���
    /// </returns>
    public virtual Task<IdentityUser?> FindByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return UserRepository.FindAsync(Guid.Parse(userId), cancellationToken: cancellationToken);
    }

    /// <summary>
    /// ���Ҳ����ؾ���ָ���淶���û������û�������У���
    /// </summary>
    /// <param name="normalizedUserName">Ҫ�����Ĺ淶���û�����</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>
    ///��ʾ�첽������ <see cref="Task"/>��������ָ�� <paramref name="normalizedUserName"/> ƥ����û���������ڣ���
    /// </returns>
    public virtual Task<IdentityUser?> FindByNameAsync([NotNull] string normalizedUserName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(normalizedUserName, nameof(normalizedUserName));
        return UserRepository.FindByNormalizedUserNameAsync(normalizedUserName, includeDetails: false, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Ϊ�û����������ϣ��
    /// </summary>
    /// <param name="user">Ϊ�����������ϣ���û���</param>
    /// <param name="passwordHash">Ҫ���õ������ϣ��</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>�����첽������ <see cref="Task"/>��</returns>
    public virtual Task SetPasswordHashAsync([NotNull] IdentityUser user, string? passwordHash, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        Check.NotNull(passwordHash, nameof(passwordHash));
        user.SetPasswordHash(passwordHash)
            .SetLastPasswordChangeTime(DateTimeOffset.Now);
        return Task.CompletedTask;
    }

    /// <summary>
    ///��ȡ�û��������ϣ��
    /// </summary>
    /// <param name="user">Ҫ���������ϣ���û���</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>�����û������ϣֵ�� <see cref="Task{TResult}"/>��</returns>
    public virtual Task<string?> GetPasswordHashAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        return Task.FromResult<string?>(user.PasswordHash);
    }

    /// <summary>
    ///����һ����־��ָʾָ�����û��Ƿ������롣
    /// </summary>
    /// <param name="user">Ҫ���������ϣ���û���</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns><see cref="Task{TResult}"/> ����һ����־��ָʾָ���û��Ƿ������롣���
    /// �û������룬�򷵻�ֵΪ true������Ϊ false��</returns>
    public virtual Task<bool> HasPasswordAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        return Task.FromResult(user.PasswordHash != null);
    }

    /// <summary>
    /// �������� <paramref name="normalizedRoleName"/> ���ӵ�ָ���� <paramref name="user"/>��
    /// </summary>
    /// <param name="user">Ҫ���ӽ�ɫ���û���</param>
    /// <param name="normalizedRoleName">Ҫ���ӵĽ�ɫ��</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>�����첽������ <see cref="Task"/>��</returns>
    public virtual async Task AddToRoleAsync([NotNull] IdentityUser user, [NotNull] string normalizedRoleName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        Check.NotNull(normalizedRoleName, nameof(normalizedRoleName));
        if (await IsInRoleAsync(user, normalizedRoleName, cancellationToken))
        {
            return;
        }
        var role = await RoleRepository.FindByNormalizedNameAsync(normalizedRoleName, cancellationToken: cancellationToken);
        if (role == null)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "��ɫ {0} �����ڣ�", normalizedRoleName));
        }
        await UserRepository.EnsureCollectionLoadedAsync(user, u => u.Roles, cancellationToken);
        user.AddRole(role.Id);
    }

    /// <summary>
    /// ��ָ���� <paramref name="user"/> ��ɾ�������� <paramref name="normalizedRoleName"/>��
    /// </summary>
    /// <param name="user">Ҫɾ�����ɫ���û���</param>
    /// <param name="normalizedRoleName">Ҫɾ���Ľ�ɫ��</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>�����첽������ <see cref="Task"/>��</returns>
    public virtual async Task RemoveFromRoleAsync([NotNull] IdentityUser user, [NotNull] string normalizedRoleName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        Check.NotNullOrWhiteSpace(normalizedRoleName, nameof(normalizedRoleName));
        var role = await RoleRepository.FindByNormalizedNameAsync(normalizedRoleName, cancellationToken: cancellationToken);
        if (role == null)
        {
            return;
        }
        await UserRepository.EnsureCollectionLoadedAsync(user, u => u.Roles, cancellationToken);
        user.RemoveRole(role.Id);
    }

    /// <summary>
    /// ����ָ�� <paramref name="user"/> �����Ľ�ɫ��
    /// </summary>
    /// <param name="user">Ӧ�������ɫ���û���</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns><see cref="Task{TResult}"/> �����û������Ľ�ɫ��</returns>
    public virtual async Task<IList<string>> GetRolesAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        var userRoles = await UserRepository.GetRoleNamesAsync(user.Id, cancellationToken: cancellationToken);
        var userOrganizationUnitRoles = await UserRepository.GetRoleNamesInOrganizationUnitAsync(user.Id, cancellationToken: cancellationToken);
        return userRoles.Union(userOrganizationUnitRoles).ToList();
    }

    /// <summary>
    ///����һ����־��ָʾָ���û��Ƿ��Ǹ��� <paramref name="normalizedRoleName"/> �ĳ�Ա��
    /// </summary>
    /// <param name="user">Ӧ������ɫ��Ա�ʸ���û���</param>
    /// <param name="normalizedRoleName">����Ա�ʸ�Ľ�ɫ</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns><see cref="Task{TResult}"/> ����һ����־��ָʾָ���û��Ƿ��Ǹ�����ĳ�Ա�����
    /// �û��Ǹ���ĳ�Ա���򷵻�ֵΪ true������Ϊ false��</returns>
    public virtual async Task<bool> IsInRoleAsync([NotNull] IdentityUser user,[NotNull] string normalizedRoleName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        Check.NotNullOrWhiteSpace(normalizedRoleName, nameof(normalizedRoleName));
        var roles = await GetRolesAsync(user, cancellationToken);
        return roles
            .Select(r => LookupNormalizer.NormalizeName(r))
            .Contains(normalizedRoleName);
    }

    /// <summary>
    ///��Ϊ�첽������ȡ��ָ�� <paramref name="user"/> ������������
    /// </summary>
    /// <param name="user">Ӧ�������������û���</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>���������û��������� <see cref="Task{TResult}"/>��</returns>
    public virtual async Task<IList<Claim>> GetClaimsAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        await UserRepository.EnsureCollectionLoadedAsync(user, u => u.Claims, cancellationToken);
        return user.Claims.Select(c => c.ToClaim()).ToList();
    }

    /// <summary>
    /// ���Ӹ���ָ�� <paramref name="user"/> �� <paramref name="claims"/>��
    /// </summary>
    /// <param name="user">Ҫ�����������û���</param>
    /// <param name="claims">���û����ӵ�������</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>�����첽������ <see cref="Task"/>��</returns>
    public virtual async Task AddClaimsAsync([NotNull] IdentityUser user, [NotNull] IEnumerable<Claim> claims, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        Check.NotNull(claims, nameof(claims));
        await UserRepository.EnsureCollectionLoadedAsync(user, u => u.Claims, cancellationToken);
        user.AddClaims(GuidGenerator, claims);
    }

    /// <summary>
    /// ��ָ�� <paramref name="user"/> �ϵ� <paramref name="claim"/> �滻Ϊ <paramref name="newClaim"/>��
    /// </summary>
    /// <param name="user">Ҫ�滻�������û���</param>
    /// <param name="claim">Ҫ�滻��������</param>
    /// <param name="newClaim">�µ�����ȡ���� <paramref name="claim"/>.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>�����첽������ <see cref="Task"/>��</returns>
    public virtual async Task ReplaceClaimAsync([NotNull] IdentityUser user, [NotNull] Claim claim, [NotNull] Claim newClaim, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        Check.NotNull(claim, nameof(claim));
        Check.NotNull(newClaim, nameof(newClaim));
        await UserRepository.EnsureCollectionLoadedAsync(user, u => u.Claims, cancellationToken);
        user.ReplaceClaim(claim, newClaim);
    }

    /// <summary>
    /// ��ָ���� <paramref name="user"/> ��ɾ�������� <paramref name="claims"/>��
    /// </summary>
    /// <param name="user">Ҫɾ���������û���</param>
    /// <param name="claims">Ҫ��ɾ����������</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>�����첽������ <see cref="Task"/>��</returns>
    public virtual async Task RemoveClaimsAsync([NotNull] IdentityUser user, [NotNull] IEnumerable<Claim> claims, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        Check.NotNull(claims, nameof(claims));
        await UserRepository.EnsureCollectionLoadedAsync(user, u => u.Claims, cancellationToken);
        user.RemoveClaims(claims);
    }

    /// <summary>
    /// ���Ӹ��� <paramref name="login"/> ��ָ���� <paramref name="user"/>��
    /// </summary>
    /// <param name="user">Ҫ���ӵ�¼���û���</param>
    /// <param name="login">Ҫ���ӵ��û��ĵ�¼����</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>�����첽������ <see cref="Task"/>��</returns>
    public virtual async Task AddLoginAsync([NotNull] IdentityUser user, [NotNull] UserLoginInfo login, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        Check.NotNull(login, nameof(login));
        await UserRepository.EnsureCollectionLoadedAsync(user, u => u.Logins, cancellationToken);
        user.AddLogin(login);
    }

    /// <summary>
    /// ��ָ���� <paramref name="user"/> ��ɾ�������� <paramref name="loginProvider"/>��
    /// </summary>
    /// <param name="user">Ҫɾ�����û���¼��</param>
    /// <param name="loginProvider">Ҫ���û���ɾ���ĵ�¼����</param>
    /// <param name="providerKey"><paramref name="loginProvider"/> �ṩ������ʶ���û�����Կ��</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>�����첽������ <see cref="Task"/>��</returns>
    public virtual async Task RemoveLoginAsync([NotNull] IdentityUser user, [NotNull] string loginProvider, [NotNull] string providerKey, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        Check.NotNull(loginProvider, nameof(loginProvider));
        Check.NotNull(providerKey, nameof(providerKey));
        await UserRepository.EnsureCollectionLoadedAsync(user, u => u.Logins, cancellationToken);
        user.RemoveLogin(loginProvider, providerKey);
    }

    /// <summary>
    /// ����ָ�� <param ref="user"/> ����ص�¼��Ϣ��
    /// </summary>
    /// <param name="user">Ҫ�����������¼��Ϣ���û���</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>
    /// �첽������ <see cref="Task"/>������ָ�� <paramref name="user"/> �� <see cref="UserLoginInfo"/> �б�������У���
    /// </returns>
    public virtual async Task<IList<UserLoginInfo>> GetLoginsAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        await UserRepository.EnsureCollectionLoadedAsync(user, u => u.Logins, cancellationToken);
        return user.Logins.Select(l => l.ToUserLoginInfo()).ToList();
    }

    /// <summary>
    /// ������ָ����¼�ṩ����͵�¼�ṩ������Կ�������û���
    /// </summary>
    /// <param name="loginProvider">�ṩ <paramref name="providerKey"/> �ĵ�¼�ṩ�̡�</param>
    /// <param name="providerKey"><paramref name="loginProvider"/> �ṩ������ʶ���û�����Կ��</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>
    /// �첽������ <see cref="Task"/>��������ָ����¼�ṩ�������Կƥ����û�������У���
    /// </returns>
    public virtual Task<IdentityUser?> FindByLoginAsync([NotNull] string loginProvider, [NotNull] string providerKey, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(loginProvider, nameof(loginProvider));
        Check.NotNull(providerKey, nameof(providerKey));
        return UserRepository.FindByLoginAsync(loginProvider, providerKey, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// ��ȡһ����־��ָʾָ�� <paramref name="user"/> �ĵ����ʼ���ַ�Ƿ��Ѿ���֤����������ʼ���ַ�Ѿ���֤��Ϊ true������Ϊ false��
    /// </summary>
    /// <param name="user">Ӧ����������ʼ�ȷ��״̬���û���</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>
    /// �����첽����������������һ����־��ָʾָ�� <paramref name="user"/> �ĵ����ʼ���ַ�Ƿ��Ѿ�ȷ�ϡ�
    /// </returns>
    public virtual Task<bool> GetEmailConfirmedAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        return Task.FromResult(user.EmailConfirmed);
    }

    /// <summary>
    /// ���ñ�־��ָ??ʾָ����<paramref name="user"/>�ĵ����ʼ���ַ�Ƿ��Ѿ�ȷ�ϡ�
    /// </summary>
    /// <param name="user">Ӧ����������ʼ�ȷ��״̬���û���</param>
    /// <param name="confirmed">A flag indicating if the email address has been confirmed, true if the address is confirmed otherwise false.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>�����첽�������������</returns>
    public virtual Task SetEmailConfirmedAsync([NotNull] IdentityUser user, bool confirmed, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        user.SetEmailConfirmed(confirmed);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Ϊ <paramref name="user"/> ���� <paramref name="email"/> ��ַ��
    /// </summary>
    /// <param name="user">Ӧ����������ʼ����û���</param>
    /// <param name="email">Ҫ���õĵ����ʼ���</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>�����첽�������������</returns>
    public virtual Task SetEmailAsync([NotNull] IdentityUser user, [NotNull] string? email, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        Check.NotNull(email, nameof(email));
        user.Email = email;
        return Task.CompletedTask;
    }

    /// <summary>
    /// ��ȡָ�� <paramref name="user"/> �ĵ����ʼ���ַ��
    /// </summary>
    /// <param name="user">Ӧ����������ʼ����û���</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>�����첽����������������Ϊָ����<paramref name="user"/>�ṩ�ʼ���ַ��</returns>
    public virtual Task<string?> GetEmailAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        return Task.FromResult<string?>(user.Email);
    }

    /// <summary>
    /// ����ָ�� <paramref name="user"/> �Ĺ淶�������ʼ���
    /// </summary>
    /// <param name="user">Ҫ����������ʼ���ַ���û���</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>
    /// �����첽���Ҳ����Ľ������������Լ���ָ���û������Ĺ淶�������ʼ���ַ������У���
    /// </returns>
    public virtual Task<string?> GetNormalizedEmailAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        return Task.FromResult<string?>(user.NormalizedEmail);
    }

    /// <summary>
    /// Ϊָ���� <paramref name="user"/> ���ù淶���ĵ����ʼ���
    /// </summary>
    /// <param name="user">Ҫ���õ����ʼ���ַ���û���</param>
    /// <param name="normalizedEmail">Ϊָ�� <paramref name="user"/> ���õĹ淶�������ʼ���</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>�����첽�������������</returns>
    public virtual Task SetNormalizedEmailAsync([NotNull] IdentityUser user, string? normalizedEmail, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        Check.NotNull(normalizedEmail, nameof(normalizedEmail));
        user.NormalizedEmail = normalizedEmail;
        return Task.CompletedTask;
    }

    /// <summary>
    /// ��ȡ��ָ���Ĺ淶�������ʼ���ַ�������û�������У���
    /// </summary>
    /// <param name="normalizedEmail">�����û��Ĺ淶�������ʼ���ַ��</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>
    /// �����첽���Ҳ����������������������ָ���Ĺ淶�������ʼ���ַ��������û���
    /// </returns>
    public virtual Task<IdentityUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return UserRepository.FindByNormalizedEmailAsync(normalizedEmail, includeDetails: false, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// ��ȡ�û��ϴ��������ڵ����һ�� <see cref="DateTimeOffset"/>������У�����ȥ���κ�ʱ�䶼Ӧ��ʾ�û�δ��������
    /// </summary>
    /// <param name="user">Ӧ�������������ڵ��û���</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>
    ///<see cref="Task{TResult}"/> ��ʾ�첽��ѯ�Ľ����<see cref="DateTimeOffset"/> �����û������ϴι��ڵ�ʱ�䣨����У���
    /// </returns>
    public virtual Task<DateTimeOffset?> GetLockoutEndDateAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        return Task.FromResult(user.LockoutEnd);
    }

    /// <summary>
    /// �����û���ֱ��ָ���Ľ������ڹ�ȥ�����ù�ȥ�Ľ������ڻ����������û���
    /// </summary>
    /// <param name="user">Ӧ�����������ڵ��û���</param>
    /// <param name="lockoutEnd"><see cref="DateTimeOffset"/> ֮�� <paramref name="user"/> ������Ӧ�ý�����</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>�����첽������ <see cref="Task"/>��</returns>
    public virtual Task SetLockoutEndDateAsync([NotNull] IdentityUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        user.LockoutEnd = lockoutEnd;
        return Task.CompletedTask;
    }

    /// <summary>
    ///��¼������ʧ�ܵķ��ʣ�������ʧ�ܷ��ʼ�����
    /// </summary>
    /// <param name="user">Ӧ����ȡ���������û���</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>�����첽������ <see cref="Task"/>���������ӵ�ʧ�ܷ��ʼ�����</returns>
    public virtual Task<int> IncrementAccessFailedCountAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        user.AccessFailedCount++;
        return Task.FromResult(user.AccessFailedCount);
    }

    /// <summary>
    /// �����û��ķ���ʧ�ܴ�����
    /// </summary>
    /// <param name="user">Ӧ���÷���ʧ�ܴ������û���</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>�����첽������ <see cref="Task"/>��</returns>
    /// <remarks>��ͨ���ڳɹ������ʻ�����á�</remarks>
    public virtual Task ResetAccessFailedCountAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        user.AccessFailedCount = 0;
        return Task.CompletedTask;
    }

    /// <summary>
    /// ����ָ�� <paramref name="user"/> �ĵ�ǰʧ�ܷ��ʼ�����
    /// </summary>
    /// <param name="user">Ӧ���������ʧ�ܴ������û���</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>�����첽������ <see cref="Task"/>������ʧ�ܵķ��ʼ�����</returns>
    public virtual Task<int> GetAccessFailedCountAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        return Task.FromResult(user.AccessFailedCount);
    }

    /// <summary>
    /// ����һ����־��ָʾ�Ƿ����Ϊָ���û������û�������
    /// </summary>
    /// <param name="user">Ӧ���ر��������û���</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>
    /// �����첽������ <see cref="Task"/>��������������û���Ϊ true������Ϊ false��
    /// </returns>
    public virtual Task<bool> GetLockoutEnabledAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        return Task.FromResult(user.LockoutEnabled);
    }

    /// <summary>
    /// ���ñ�־ָʾ�Ƿ��������ָ���� <paramref name="user"/>��
    /// </summary>
    /// <param name="user">Ӧ���ÿ��������û���</param>
    /// <param name="enabled">һ����־��ָʾ�Ƿ����Ϊָ���� <paramref name="user"/> ����������</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>�����첽������ <see cref="Task"/>��</returns>
    public virtual Task SetLockoutEnabledAsync([NotNull] IdentityUser user, bool enabled, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        user.LockoutEnabled = enabled;
        return Task.CompletedTask;
    }

    /// <summary>
    /// Ϊָ����<paramref name="user"/>���õ绰���롣
    /// </summary>
    /// <param name="user">��Ҫ���õ绰������û���</param>
    /// <param name="phoneNumber">Ҫ���õĵ绰���롣</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>�����첽������ <see cref="Task"/>��</returns>
    public virtual Task SetPhoneNumberAsync([NotNull] IdentityUser user, string? phoneNumber, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        Check.NotNull(phoneNumber, nameof(phoneNumber));
        user.PhoneNumber = phoneNumber;
        return Task.CompletedTask;
    }

    /// <summary>
    ///��ȡָ�� <paramref name="user"/> �ĵ绰���루����У���
    /// </summary>
    /// <param name="user">��Ҫ������绰������û���</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>�����첽������ <see cref="Task"/>�������û��ĵ绰���루����У���</returns>
    public virtual Task<string?> GetPhoneNumberAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        return Task.FromResult<string?>(user.PhoneNumber);
    }

    /// <summary>
    /// ��ȡһ����־��ָʾָ��<paramref name="user"/>�ĵ绰�����Ƿ��Ѿ�ȷ�ϡ�
    /// </summary>
    /// <param name="user">���û�����һ����־���������ǵĵ绰�����Ƿ���ȷ�ϡ�</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>
    /// �����첽������<see cref="Task"/>�����ָ����<paramref name="user"/>����ȷ�ϵĵ绰�����򷵻�true�����򷵻�false��
    /// </returns>
    public virtual Task<bool> GetPhoneNumberConfirmedAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        return Task.FromResult(user.PhoneNumberConfirmed);
    }

    /// <summary>
    /// ����һ����־��ָʾָ���� <paramref name="user"/> �ĵ绰�����Ƿ��Ѿ�ȷ�ϡ�
    /// </summary>
    /// <param name="user">��Ҫ���õ绰����ȷ��״̬���û���</param>
    /// <param name="confirmed">ָʾ�û��ĵ绰�����Ƿ��Ѿ�ȷ�ϵı�־��</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>�����첽������ <see cref="Task"/>��</returns>
    public virtual Task SetPhoneNumberConfirmedAsync([NotNull] IdentityUser user, bool confirmed, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        user.SetPhoneNumberConfirmed(confirmed);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Ϊָ���� <paramref name="user"/> �����ṩ�İ�ȫ <paramref name="stamp"/>��
    /// </summary>
    /// <param name="user">Ӧ���ð�ȫ���ǵ��û���</param>
    /// <param name="stamp">Ҫ���õİ�ȫӡ�¡�</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>�����첽������ <see cref="Task"/>��</returns>
    public virtual Task SetSecurityStampAsync([NotNull] IdentityUser user, string stamp, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        user.SecurityStamp = stamp;
        return Task.CompletedTask;
    }

    /// <summary>
    ///��ȡָ�� <paramref name="user" /> �İ�ȫӡ�¡�
    /// </summary>
    /// <param name="user">Ӧ���ð�ȫ���ǵ��û���</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>�����첽������ <see cref="Task"/>������ָ�� <paramref name="user"/> �İ�ȫ����</returns>
    public virtual Task<string?> GetSecurityStampAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        return Task.FromResult<string?>(user.SecurityStamp);
    }

    /// <summary>
    /// ����һ����־��ָʾָ���� <paramref name="user"/> �Ƿ�������˫����������֤����Ϊ�첽������
    /// </summary>
    /// <param name="user">Ӧ����˫����������֤����״̬���û���</param>
    /// <param name="enabled">һ����־��ָʾָ���� <paramref name="user"/> �Ƿ�������˫����������֤��</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>�����첽������ <see cref="Task"/>��</returns>
    public virtual Task SetTwoFactorEnabledAsync([NotNull] IdentityUser user, bool enabled, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        user.TwoFactorEnabled = enabled;
        return Task.CompletedTask;
    }

    /// <summary>
    /// ����һ����־��ָʾָ���� <paramref name="user"/> �Ƿ�������˫����������֤����Ϊ�첽������
    /// </summary>
    /// <param name="user">Ӧ����˫����������֤����״̬���û���</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>
    /// �����첽������ <see cref="Task"/>������һ����־��ָʾָ���� <paramref name="user"/> �Ƿ�������˫����������֤��
    /// </returns>
    public virtual Task<bool> GetTwoFactorEnabledAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        return Task.FromResult(user.TwoFactorEnabled);
    }

    /// <summary>
    /// ��������ָ�������������û���
    /// </summary>
    /// <param name="claim">Ӧ�������û���������</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>
    /// <see cref="Task"/> ��������ָ���������û��б�������У���
    /// </returns>
    public virtual async Task<IList<IdentityUser>> GetUsersForClaimAsync([NotNull] Claim claim, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(claim, nameof(claim));
        return await UserRepository.GetListByClaimAsync(claim, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// ����ָ����ɫ�������û���
    /// </summary>
    /// <param name="normalizedRoleName">Ӧ�������û��Ľ�ɫ��</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>
    /// <see cref="Task"/> ��������ָ����ɫ���û��б�������У���
    /// </returns>
    public virtual async Task<IList<IdentityUser>> GetUsersInRoleAsync([NotNull] string normalizedRoleName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrEmpty(normalizedRoleName))
        {
            throw new ArgumentNullException(nameof(normalizedRoleName));
        }
        return await UserRepository.GetListByNormalizedRoleNameAsync(normalizedRoleName, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Ϊ�ض��û���������ֵ��
    /// </summary>
    /// <param name="user">�û���</param>
    /// <param name="loginProvider">���Ƶ�������֤�ṩ�̡�</param>
    /// <param name="name">���Ƶ����ơ�</param>
    /// <param name="value">���Ƶļ�ֵ��</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>�����첽������ <see cref="Task"/>��</returns>
    public virtual async Task SetTokenAsync([NotNull] IdentityUser user, string loginProvider, string name, string? value, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        Check.NotNull(value, nameof(value));
        await UserRepository.EnsureCollectionLoadedAsync(user, u => u.Tokens, cancellationToken);
        user.SetToken(loginProvider, name, value);
    }

    /// <summary>
    /// ɾ���û������ơ�
    /// </summary>
    /// <param name="user">�û���</param>
    /// <param name="loginProvider">���Ƶ�������֤�ṩ�̡�</param>
    /// <param name="name">���Ƶ����ơ�</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>�����첽������ <see cref="Task"/>��</returns>
    public virtual async Task RemoveTokenAsync(IdentityUser user, string loginProvider, string name, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        await UserRepository.EnsureCollectionLoadedAsync(user, u => u.Tokens, cancellationToken);
        user.RemoveToken(loginProvider, name);
    }

    /// <summary>
    ///��������ֵ��
    /// </summary>
    /// <param name="user">�û���</param>
    /// <param name="loginProvider">���Ƶ�������֤�ṩ�̡�</param>
    /// <param name="name">���Ƶ����ơ�</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>�����첽������ <see cref="Task"/>��</returns>
    public virtual async Task<string?> GetTokenAsync(IdentityUser user, string loginProvider, string name, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Check.NotNull(user, nameof(user));
        await UserRepository.EnsureCollectionLoadedAsync(user, u => u.Tokens, cancellationToken);
        return user.FindToken(loginProvider, name)?.Value;
    }
    /// <summary>
    /// ����������֤����Կ
    /// </summary>
    /// <param name="user"></param>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task SetAuthenticatorKeyAsync(IdentityUser user, string key, CancellationToken cancellationToken = default)
    {
        return SetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, key, cancellationToken);
    }

    /// <summary>
    /// ��ȡ������֤����Կ
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task<string?> GetAuthenticatorKeyAsync(IdentityUser user, CancellationToken cancellationToken = default)
    {
        return GetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, cancellationToken);
    }

    /// <summary>
    /// ���ض����û���˵���ж��ٻָ�������Ч��
    /// </summary>
    /// <param name="user">ӵ�лָ�������û���</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>�û�����Ч�ָ������������</returns>
    public virtual async Task<int> CountCodesAsync(IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));

        var mergedCodes = await GetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, cancellationToken) ?? "";
        if (mergedCodes.Length > 0)
        {
            return mergedCodes.Split(';').Length;
        }

        return 0;
    }

    /// <summary>
    /// �����û��Ļָ����룬ͬʱʹ������ǰ�Ļָ�������Ч��
    /// </summary>
    /// <param name="user">�洢�»ָ�������û���</param>
    /// <param name="recoveryCodes">�û����»ָ����롣</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>�û����»ָ����롣</returns>
    public virtual Task ReplaceCodesAsync(IdentityUser user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken = default)
    {
        var mergedCodes = string.Join(";", recoveryCodes);
        return SetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, mergedCodes, cancellationToken);
    }

    /// <summary>
    ///���ػָ�������û��Ƿ���Ч��ע�⣺�ָ��������Чһ�Σ�ʹ�ú�ʧЧ��
    /// </summary>
    /// <param name="user">ӵ�лָ�������û���</param>
    /// <param name="code">Ҫʹ�õĻָ����롣</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> ���ڴ���Ӧȡ��������֪ͨ��</param>
    /// <returns>���Ϊ�û��ҵ��ָ�������Ϊ True��</returns>
    public virtual async Task<bool> RedeemCodeAsync(IdentityUser user, string code, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));
        Check.NotNull(code, nameof(code));

        var mergedCodes = await GetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, cancellationToken) ?? "";
        var splitCodes = mergedCodes.Split(';');
        if (splitCodes.Contains(code))
        {
            var updatedCodes = new List<string>(splitCodes.Where(s => s != code));
            await ReplaceCodesAsync(user, updatedCodes, cancellationToken);
            return true;
        }
        return false;
    }

    /// <summary>
    /// �ڲ���¼�ṩ��
    /// </summary>
    /// <returns></returns>
    public virtual Task<string> GetInternalLoginProviderAsync()
    {
        return Task.FromResult(InternalLoginProvider);
    }

    /// <summary>
    /// AuthenticatorKeyToken����
    /// </summary>
    /// <returns></returns>
    public virtual Task<string> GetAuthenticatorKeyTokenNameAsync()
    {
        return Task.FromResult(AuthenticatorKeyTokenName);
    }

    /// <summary>
    /// �ָ�������������
    /// </summary>
    /// <returns></returns>
    public virtual Task<string> GetRecoveryCodeTokenNameAsync()
    {
        return Task.FromResult(RecoveryCodeTokenName);
    }

    /// <summary>
    /// 
    /// </summary>
    public virtual void Dispose()
    {

    }
}
