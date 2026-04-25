using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Censeq.Identity.Entities;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Timing;

namespace Censeq.Identity;

/// <summary>
/// 表示指定用户和角色类型的持久化存储的新实例。
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
    /// 获取或设置当前操作发生错误时使用的 <see cref="IdentityErrorDescriber"/>。
    /// </summary>
    public IdentityErrorDescriber ErrorDescriber { get; set; }

    /// <summary>
    /// 获取或设置一个标志，指示是否在调用 CreateAsync、UpdateAsync 和 DeleteAsync 后自动持久化更改。
    /// </summary>
    /// <value>
    /// 如果应自动持久化更改则为 true，否则为 false。
    /// </value>
    public bool AutoSaveChanges { get; set; } = true;

    /// <summary>
    /// I身份角色仓储
    /// </summary>
    protected IIdentityRoleRepository RoleRepository { get; }
    /// <summary>
    /// IGuidGenerator
    /// </summary>
    protected IGuidGenerator GuidGenerator { get; }
    /// <summary>
    /// ILogger<Identity角色Store>
    /// </summary>
    protected ILogger<IdentityRoleStore> Logger { get; }
    /// <summary>
    /// I查找Normalizer
    /// </summary>
    protected ILookupNormalizer LookupNormalizer { get; }
    /// <summary>
    /// I身份用户仓储
    /// </summary>
    protected IIdentityUserRepository UserRepository { get; }

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
    /// 获取指定 <paramref name="user"/> 的用户标识符。
    /// </summary>
    /// <param name="user">要获取其标识符的用户。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>表示异步操作的 <see cref="Task"/>，包含指定 <paramref name="user"/> 的标识符。</returns>
    public virtual Task<string> GetUserIdAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));

        return Task.FromResult(user.Id.ToString());
    }

    /// <summary>
    /// 获取指定 <paramref name="user"/> 的用户名。
    /// </summary>
    /// <param name="user">要获取其名称的用户。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>表示异步操作的 <see cref="Task"/>，包含指定 <paramref name="user"/> 的名称。</returns>
    public virtual Task<string?> GetUserNameAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));

        return Task.FromResult<string?>(user.UserName);
    }

    /// <summary>
    /// 为指定 <paramref name="user"/> 设置给定的 <paramref name="userName"/>。
    /// </summary>
    /// <param name="user">要设置其名称的用户。</param>
    /// <param name="userName">要设置的用户名。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>表示异步操作的 <see cref="Task"/>。</returns>
    public virtual Task SetUserNameAsync([NotNull] IdentityUser user, string? userName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));

        user.UserName = userName!;

        return Task.CompletedTask;
    }

    /// <summary>
    /// 获取指定 <paramref name="user"/> 的标准化用户名。
    /// </summary>
    /// <param name="user">要获取其标准化名称的用户。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>表示异步操作的 <see cref="Task"/>，包含指定 <paramref name="user"/> 的标准化用户名。</returns>
    public virtual Task<string?> GetNormalizedUserNameAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));

        return Task.FromResult<string?>(user.NormalizedUserName);
    }

    /// <summary>
    /// 为指定 <paramref name="user"/> 设置给定的标准化名称。
    /// </summary>
    /// <param name="user">要设置其名称的用户。</param>
    /// <param name="normalizedName">要设置的标准化名称。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>表示异步操作的 <see cref="Task"/>。</returns>
    public virtual Task SetNormalizedUserNameAsync([NotNull] IdentityUser user, string? normalizedName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));

        user.NormalizedUserName = normalizedName!;

        return Task.CompletedTask;
    }

    /// <summary>
    /// 在用户存储中创建指定的 <paramref name="user"/>。
    /// </summary>
    /// <param name="user">要创建的用户。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>表示异步操作的 <see cref="Task"/>，包含创建操作的 <see cref="IdentityResult"/>。</returns>
    public virtual async Task<IdentityResult> CreateAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));

        await UserRepository.InsertAsync(user, AutoSaveChanges, cancellationToken);

        return IdentityResult.Success;
    }

    /// <summary>
    /// 在用户存储中更新指定的 <paramref name="user"/>。
    /// </summary>
    /// <param name="user">要更新的用户。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>表示异步操作的 <see cref="Task"/>，包含更新操作的 <see cref="IdentityResult"/>。</returns>
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
            Logger.LogWarning(ex.ToString()); //Trigger some AbpHandledException event
            return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
        }

        return IdentityResult.Success;
    }

    /// <summary>
    /// 从用户存储中删除指定的 <paramref name="user"/>。
    /// </summary>
    /// <param name="user">要删除的用户。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>表示异步操作的 <see cref="Task"/>，包含删除操作的 <see cref="IdentityResult"/>。</returns>
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
            Logger.LogWarning(ex.ToString()); //Trigger some AbpHandledException event
            return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
        }

        return IdentityResult.Success;
    }

    /// <summary>
    /// 查找并返回具有指定 <paramref name="userId"/> 的用户（如果存在）。
    /// </summary>
    /// <param name="userId">要搜索的用户 ID。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>
    /// 表示异步操作的 <see cref="Task"/>，包含与指定 <paramref name="userId"/> 匹配的用户（如果存在）。
    /// </returns>
    public virtual async Task<IdentityUser?> FindByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await UserRepository.FindAsync(Guid.Parse(userId), cancellationToken: cancellationToken);
    }

    /// <summary>
    /// 查找并返回具有指定标准化用户名的用户（如果存在）。
    /// </summary>
    /// <param name="normalizedUserName">要搜索的标准化用户名。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>
    /// 表示异步操作的 <see cref="Task"/>，包含与指定 <paramref name="normalizedUserName"/> 匹配的用户（如果存在）。
    /// </returns>
    public virtual Task<IdentityUser?> FindByNameAsync([NotNull] string normalizedUserName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(normalizedUserName, nameof(normalizedUserName));

        return UserRepository.FindByNormalizedUserNameAsync(normalizedUserName, includeDetails: false, cancellationToken: cancellationToken)!;
    }

    /// <summary>
    /// 设置用户的密码哈希。
    /// </summary>
    /// <param name="user">要设置密码哈希的用户。</param>
    /// <param name="passwordHash">要设置的密码哈希。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>表示异步操作的 <see cref="Task"/>。</returns>
    public virtual Task SetPasswordHashAsync([NotNull] IdentityUser user, string? passwordHash, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));

        user.PasswordHash = passwordHash!;

        user.SetLastPasswordChangeTime(DateTime.UtcNow);

        return Task.CompletedTask;
    }

    /// <summary>
    /// 获取用户的密码哈希。
    /// </summary>
    /// <param name="user">要获取密码哈希的用户。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>包含用户密码哈希的 <see cref="Task{TResult}"/>。</returns>
    public virtual Task<string?> GetPasswordHashAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));

        return Task.FromResult<string?>(user.PasswordHash);
    }

    /// <summary>
    /// 返回一个标志，指示指定用户是否设置了密码。
    /// </summary>
    /// <param name="user">要检查的用户。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>包含标志的 <see cref="Task{TResult}"/>，如果指定用户已设置密码则为 true，否则为 false。</returns>
    public virtual Task<bool> HasPasswordAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));

        return Task.FromResult(user.PasswordHash != null);
    }

    /// <summary>
    /// 将指定的 <paramref name="normalizedRoleName"/> 添加到指定的 <paramref name="user"/>。
    /// </summary>
    /// <param name="user">要添加角色的用户。</param>
    /// <param name="normalizedRoleName">要添加的角色。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>表示异步操作的 <see cref="Task"/>。</returns>
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
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Role {0} does not exist!", normalizedRoleName));
        }

        await UserRepository.EnsureCollectionLoadedAsync(user, u => u.Roles, cancellationToken);

        user.AddRole(role.Id);
    }

    /// <summary>
    /// 从指定的 <paramref name="user"/> 中移除指定的 <paramref name="normalizedRoleName"/>。
    /// </summary>
    /// <param name="user">要移除角色的用户。</param>
    /// <param name="normalizedRoleName">要移除的角色。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>表示异步操作的 <see cref="Task"/>。</returns>
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
    /// 获取指定 <paramref name="user"/> 所属的角色列表。
    /// </summary>
    /// <param name="user">要获取其角色的用户。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>包含用户所属角色的 <see cref="Task{TResult}"/>。</returns>
    public virtual async Task<IList<string>> GetRolesAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));

        var userRoles = await UserRepository
            .GetRoleNamesAsync(user.Id, cancellationToken: cancellationToken);

        var userOrganizationUnitRoles = await UserRepository
            .GetRoleNamesInOrganizationUnitAsync(user.Id, cancellationToken: cancellationToken);

        return userRoles.Union(userOrganizationUnitRoles).ToList();
    }

    /// <summary>
    /// 返回一个标志，指示指定用户是否是指定 <paramref name="normalizedRoleName"/> 的成员。
    /// </summary>
    /// <param name="user">要检查其角色成员资格的用户。</param>
    /// <param name="normalizedRoleName">要检查成员资格的角色。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>包含标志的 <see cref="Task{TResult}"/>，如果指定用户是指定角色的成员则为 true，否则为 false。</returns>
    public virtual async Task<bool> IsInRoleAsync(
        [NotNull] IdentityUser user,
        [NotNull] string normalizedRoleName,
        CancellationToken cancellationToken = default)
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
    /// 以异步操作获取与指定 <paramref name="user"/> 关联的声明。
    /// </summary>
    /// <param name="user">要获取其声明的用户。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>包含授予用户的声明的 <see cref="Task{TResult}"/>。</returns>
    public virtual async Task<IList<Claim>> GetClaimsAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));

        await UserRepository.EnsureCollectionLoadedAsync(user, u => u.Claims, cancellationToken);

        return user.Claims.Select(c => c.ToClaim()).ToList();
    }

    /// <summary>
    /// 将指定的 <paramref name="claims"/> 添加到指定的 <paramref name="user"/>。
    /// </summary>
    /// <param name="user">要添加声明的用户。</param>
    /// <param name="claims">要添加到用户的声明。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>表示异步操作的 <see cref="Task"/>。</returns>
    public virtual async Task AddClaimsAsync([NotNull] IdentityUser user, [NotNull] IEnumerable<Claim> claims, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));
        Check.NotNull(claims, nameof(claims));

        await UserRepository.EnsureCollectionLoadedAsync(user, u => u.Claims, cancellationToken);

        user.AddClaims(GuidGenerator, claims);
    }

    /// <summary>
    /// 在指定的 <paramref name="user"/> 上用 <paramref name="newClaim"/> 替换 <paramref name="claim"/>。
    /// </summary>
    /// <param name="user">要替换声明的用户。</param>
    /// <param name="claim">要替换的声明。</param>
    /// <param name="newClaim">用于替换 <paramref name="claim"/> 的新声明。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>表示异步操作的 <see cref="Task"/>。</returns>
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
    /// 从指定的 <paramref name="user"/> 中移除指定的 <paramref name="claims"/>。
    /// </summary>
    /// <param name="user">要移除声明的用户。</param>
    /// <param name="claims">要移除的声明。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>表示异步操作的 <see cref="Task"/>。</returns>
    public virtual async Task RemoveClaimsAsync([NotNull] IdentityUser user, [NotNull] IEnumerable<Claim> claims, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));
        Check.NotNull(claims, nameof(claims));

        await UserRepository.EnsureCollectionLoadedAsync(user, u => u.Claims, cancellationToken);

        user.RemoveClaims(claims);
    }

    /// <summary>
    /// 将指定的 <paramref name="login"/> 添加到指定的 <paramref name="user"/>。
    /// </summary>
    /// <param name="user">要添加登录信息的用户。</param>
    /// <param name="login">要添加到用户的登录信息。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>表示异步操作的 <see cref="Task"/>。</returns>
    public virtual async Task AddLoginAsync([NotNull] IdentityUser user, [NotNull] UserLoginInfo login, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));
        Check.NotNull(login, nameof(login));

        await UserRepository.EnsureCollectionLoadedAsync(user, u => u.Logins, cancellationToken);

        user.AddLogin(login);
    }

    /// <summary>
    /// 从指定的 <paramref name="user"/> 中移除指定的 <paramref name="loginProvider"/>。
    /// </summary>
    /// <param name="user">要移除登录信息的用户。</param>
    /// <param name="loginProvider">要移除的登录提供程序。</param>
    /// <param name="providerKey">由 <paramref name="loginProvider"/> 提供的用于标识用户的密钥。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>表示异步操作的 <see cref="Task"/>。</returns>
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
    /// 获取指定 <paramref name="user"/> 的关联登录信息。
    /// </summary>
    /// <param name="user">要获取其关联登录信息的用户。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>
    /// 表示异步操作的 <see cref="Task"/>，包含指定 <paramref name="user"/> 的 <see cref="UserLoginInfo"/> 列表（如果有）。
    /// </returns>
    public virtual async Task<IList<UserLoginInfo>> GetLoginsAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));

        await UserRepository.EnsureCollectionLoadedAsync(user, u => u.Logins, cancellationToken);

        return user.Logins.Select(l => l.ToUserLoginInfo()).ToList();
    }

    /// <summary>
    /// 获取与指定登录提供程序和登录提供程序密钥关联的用户。
    /// </summary>
    /// <param name="loginProvider">提供 <paramref name="providerKey"/> 的登录提供程序。</param>
    /// <param name="providerKey">由 <paramref name="loginProvider"/> 提供的用于标识用户的密钥。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>
    /// 表示异步操作的 <see cref="Task"/>，包含与指定登录提供程序和密钥匹配的用户（如果有）。
    /// </returns>
    public virtual Task<IdentityUser?> FindByLoginAsync([NotNull] string loginProvider, [NotNull] string providerKey, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(loginProvider, nameof(loginProvider));
        Check.NotNull(providerKey, nameof(providerKey));

        return UserRepository.FindByLoginAsync(loginProvider, providerKey, cancellationToken: cancellationToken)!;
    }

    /// <summary>
    /// 获取一个标志，指示指定 <paramref name="user"/> 的电子邮箱地址是否已验证。
    /// </summary>
    /// <param name="user">要返回其邮箱确认状态的用户。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>
    /// 包含异步操作结果的任务对象，一个标志，指示指定 <paramref name="user"/> 的电子邮箱地址是否已确认。
    /// </returns>
    public virtual Task<bool> GetEmailConfirmedAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));

        return Task.FromResult(user.EmailConfirmed);
    }

    /// <summary>
    /// 设置标志，指示指定 <paramref name="user"/> 的电子邮箱地址是否已确认。
    /// </summary>
    /// <param name="user">要设置其邮箱确认状态的用户。</param>
    /// <param name="confirmed">指示电子邮箱地址是否已确认的标志，如果已确认则为 true，否则为 false。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>表示异步操作的任务对象。</returns>
    public virtual Task SetEmailConfirmedAsync([NotNull] IdentityUser user, bool confirmed, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));

        user.SetEmailConfirmed(confirmed);

        return Task.CompletedTask;
    }

    /// <summary>
    /// 为 <paramref name="user"/> 设置 <paramref name="email"/> 地址。
    /// </summary>
    /// <param name="user">要设置其邮箱的用户。</param>
    /// <param name="email">要设置的邮箱地址。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>表示异步操作的任务对象。</returns>
    public virtual Task SetEmailAsync([NotNull] IdentityUser user, string? email, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));
        Check.NotNull(email, nameof(email));

        user.Email = email;

        return Task.CompletedTask;
    }

    /// <summary>
    /// 获取指定 <paramref name="user"/> 的电子邮箱地址。
    /// </summary>
    /// <param name="user">要返回其邮箱的用户。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>包含异步操作结果的任务对象，即指定 <paramref name="user"/> 的电子邮箱地址。</returns>
    public virtual Task<string?> GetEmailAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));

        return Task.FromResult<string?>(user.Email);
    }

    /// <summary>
    /// 返回指定 <paramref name="user"/> 的标准化电子邮箱地址。
    /// </summary>
    /// <param name="user">要获取其邮箱地址的用户。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>
    /// 包含异步查找操作结果的任务对象，即与指定用户关联的标准化电子邮箱地址（如果有）。
    /// </returns>
    public virtual Task<string?> GetNormalizedEmailAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));

        return Task.FromResult<string?>(user.NormalizedEmail);
    }

    /// <summary>
    /// 为指定 <paramref name="user"/> 设置标准化电子邮箱地址。
    /// </summary>
    /// <param name="user">要设置其邮箱地址的用户。</param>
    /// <param name="normalizedEmail">要为指定 <paramref name="user"/> 设置的标准化邮箱地址。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>表示异步操作的任务对象。</returns>
    public virtual Task SetNormalizedEmailAsync([NotNull] IdentityUser user, string? normalizedEmail, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));

        user.NormalizedEmail = normalizedEmail!;

        return Task.CompletedTask;
    }

    /// <summary>
    /// 获取与指定标准化电子邮箱地址关联的用户（如果有）。
    /// </summary>
    /// <param name="normalizedEmail">要返回用户的标准化电子邮箱地址。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>
    /// 包含异步查找操作结果的任务对象，即与指定标准化电子邮箱地址关联的用户（如果有）。
    /// </returns>
    public virtual Task<IdentityUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return UserRepository.FindByNormalizedEmailAsync(normalizedEmail, includeDetails: false, cancellationToken: cancellationToken)!;
    }

    /// <summary>
    /// 获取用户上次锁定到期的时间（如果有）。
    /// 过去任何时间都表示用户未被锁定。
    /// </summary>
    /// <param name="user">要获取其锁定日期的用户。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>
    /// 表示异步查询结果的 <see cref="Task{TResult}"/>，包含用户上次锁定到期的 <see cref="DateTimeOffset"/>（如果有）。
    /// </returns>
    public virtual Task<DateTimeOffset?> GetLockoutEndDateAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));

        return Task.FromResult(user.LockoutEnd);
    }

    /// <summary>
    /// 将用户锁定直到指定的结束日期过去。将结束日期设置为过去的时间会立即解锁用户。
    /// </summary>
    /// <param name="user">要设置其锁定日期的用户。</param>
    /// <param name="lockoutEnd"><paramref name="user"/> 的锁定应结束的 <see cref="DateTimeOffset"/>。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>表示异步操作的 <see cref="Task"/>。</returns>
    public virtual Task SetLockoutEndDateAsync([NotNull] IdentityUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));

        user.LockoutEnd = lockoutEnd;

        return Task.CompletedTask;
    }

    /// <summary>
    /// 记录一次失败的访问，增加失败访问计数。
    /// </summary>
    /// <param name="user">要增加其失败访问计数的用户。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>表示异步操作的 <see cref="Task"/>，包含增加后的失败访问计数。</returns>
    public virtual Task<int> IncrementAccessFailedCountAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));

        user.AccessFailedCount++;

        return Task.FromResult(user.AccessFailedCount);
    }

    /// <summary>
    /// 重置用户的失败访问计数。
    /// </summary>
    /// <param name="user">要重置其失败访问计数的用户。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>表示异步操作的 <see cref="Task"/>。</returns>
    /// <remarks>通常在账户成功访问后调用。</remarks>
    public virtual Task ResetAccessFailedCountAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));

        user.AccessFailedCount = 0;

        return Task.CompletedTask;
    }

    /// <summary>
    /// 检索指定 <paramref name="user"/> 的当前失败访问计数。
    /// </summary>
    /// <param name="user">要获取其失败访问计数的用户。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>表示异步操作的 <see cref="Task"/>，包含失败访问计数。</returns>
    public virtual Task<int> GetAccessFailedCountAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));

        return Task.FromResult(user.AccessFailedCount);
    }

    /// <summary>
    /// 检索一个标志，指示是否可以为指定用户启用锁定。
    /// </summary>
    /// <param name="user">要返回其可锁定状态的用户。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>
    /// 表示异步操作的 <see cref="Task"/>，如果用户可以锁定则为 true，否则为 false。
    /// </returns>
    public virtual Task<bool> GetLockoutEnabledAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));

        return Task.FromResult(user.LockoutEnabled);
    }

    /// <summary>
    /// 设置标志，指示是否可以为指定 <paramref name="user"/> 启用锁定。
    /// </summary>
    /// <param name="user">要设置其可锁定状态的用户。</param>
    /// <param name="enabled">指示是否可以为指定 <paramref name="user"/> 启用锁定的标志。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>表示异步操作的 <see cref="Task"/>。</returns>
    public virtual Task SetLockoutEnabledAsync([NotNull] IdentityUser user, bool enabled, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));

        user.LockoutEnabled = enabled;

        return Task.CompletedTask;
    }

    /// <summary>
    /// 为指定 <paramref name="user"/> 设置电话号码。
    /// </summary>
    /// <param name="user">要设置其电话号码的用户。</param>
    /// <param name="phoneNumber">要设置的电话号码。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>表示异步操作的 <see cref="Task"/>。</returns>
    public virtual Task SetPhoneNumberAsync([NotNull] IdentityUser user, string? phoneNumber, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));

        user.PhoneNumber = phoneNumber!;

        return Task.CompletedTask;
    }

    /// <summary>
    /// 获取指定 <paramref name="user"/> 的电话号码（如果有）。
    /// </summary>
    /// <param name="user">要获取其电话号码的用户。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>表示异步操作的 <see cref="Task"/>，包含用户的电话号码（如果有）。</returns>
    public virtual Task<string?> GetPhoneNumberAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));

        return Task.FromResult<string?>(user.PhoneNumber);
    }

    /// <summary>
    /// 获取一个标志，指示指定 <paramref name="user"/> 的电话号码是否已确认。
    /// </summary>
    /// <param name="user">要返回其电话号码确认状态标志的用户。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>
    /// 表示异步操作的 <see cref="Task"/>，如果指定 <paramref name="user"/> 的电话号码已确认则返回 true，否则返回 false。
    /// </returns>
    public virtual Task<bool> GetPhoneNumberConfirmedAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));

        return Task.FromResult(user.PhoneNumberConfirmed);
    }

    /// <summary>
    /// 设置标志，指示指定 <paramref name="user"/> 的电话号码是否已确认。
    /// </summary>
    /// <param name="user">要设置其电话号码确认状态的用户。</param>
    /// <param name="confirmed">指示用户电话号码是否已确认的标志。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>表示异步操作的 <see cref="Task"/>。</returns>
    public virtual Task SetPhoneNumberConfirmedAsync([NotNull] IdentityUser user, bool confirmed, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));

        user.SetPhoneNumberConfirmed(confirmed);

        return Task.CompletedTask;
    }

    /// <summary>
    /// 为指定 <paramref name="user"/> 设置提供的安全戳 <paramref name="stamp"/>。
    /// </summary>
    /// <param name="user">要设置其安全戳的用户。</param>
    /// <param name="stamp">要设置的安全戳。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>表示异步操作的 <see cref="Task"/>。</returns>
    public virtual Task SetSecurityStampAsync([NotNull] IdentityUser user, string stamp, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));

        user.SecurityStamp = stamp;

        return Task.CompletedTask;
    }

    /// <summary>
    /// 获取指定 <paramref name="user"/> 的安全戳。
    /// </summary>
    /// <param name="user">要获取其安全戳的用户。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>表示异步操作的 <see cref="Task"/>，包含指定 <paramref name="user"/> 的安全戳。</returns>
    public virtual Task<string?> GetSecurityStampAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));

        return Task.FromResult<string?>(user.SecurityStamp);
    }

    /// <summary>
    /// 以异步操作设置标志，指示指定 <paramref name="user"/> 是否启用了双重身份验证。
    /// </summary>
    /// <param name="user">要设置其双重身份验证启用状态的用户。</param>
    /// <param name="enabled">指示指定 <paramref name="user"/> 是否启用了双重身份验证的标志。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>表示异步操作的 <see cref="Task"/>。</returns>
    public virtual Task SetTwoFactorEnabledAsync([NotNull] IdentityUser user, bool enabled, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));

        user.TwoFactorEnabled = enabled;

        return Task.CompletedTask;
    }

    /// <summary>
    /// 以异步操作返回一个标志，指示指定 <paramref name="user"/> 是否启用了双重身份验证。
    /// </summary>
    /// <param name="user">要获取其双重身份验证启用状态的用户。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>
    /// 表示异步操作的 <see cref="Task"/>，包含一个标志，指示指定 <paramref name="user"/> 是否启用了双重身份验证。
    /// </returns>
    public virtual Task<bool> GetTwoFactorEnabledAsync([NotNull] IdentityUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));

        return Task.FromResult(user.TwoFactorEnabled);
    }

    /// <summary>
    /// 检索具有指定声明的所有用户。
    /// </summary>
    /// <param name="claim">要检索其用户的声明。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>
    /// 包含具有指定声明的用户列表（如果有）的 <see cref="Task"/>。
    /// </returns>
    public virtual async Task<IList<IdentityUser>> GetUsersForClaimAsync([NotNull] Claim claim, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(claim, nameof(claim));

        return await UserRepository.GetListByClaimAsync(claim, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// 检索指定角色中的所有用户。
    /// </summary>
    /// <param name="normalizedRoleName">要检索其用户的角色。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>
    /// 包含指定角色中的用户列表（如果有）的 <see cref="Task"/>。
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
    /// 为特定用户设置令牌值。
    /// </summary>
    /// <param name="user">用户。</param>
    /// <param name="loginProvider">令牌的认证提供程序。</param>
    /// <param name="name">令牌的名称。</param>
    /// <param name="value">令牌的值。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>表示异步操作的 <see cref="Task"/>。</returns>
    public virtual async Task SetTokenAsync([NotNull] IdentityUser user, string loginProvider, string name, string? value, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));

        await UserRepository.EnsureCollectionLoadedAsync(user, u => u.Tokens, cancellationToken);

        user.SetToken(loginProvider, name, value!);
    }

    /// <summary>
    /// 删除用户的令牌。
    /// </summary>
    /// <param name="user">用户。</param>
    /// <param name="loginProvider">令牌的认证提供程序。</param>
    /// <param name="name">令牌的名称。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>表示异步操作的 <see cref="Task"/>。</returns>
    public virtual async Task RemoveTokenAsync(IdentityUser user, string loginProvider, string name, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));

        await UserRepository.EnsureCollectionLoadedAsync(user, u => u.Tokens, cancellationToken);

        user.RemoveToken(loginProvider, name);
    }

    /// <summary>
    /// 返回令牌值。
    /// </summary>
    /// <param name="user">用户。</param>
    /// <param name="loginProvider">令牌的认证提供程序。</param>
    /// <param name="name">令牌的名称。</param>
    /// <param name="cancellationToken">用于传播操作取消通知的 <see cref="CancellationToken"/>。</param>
    /// <returns>表示异步操作的 <see cref="Task"/>。</returns>
    public virtual async Task<string?> GetTokenAsync(IdentityUser user, string loginProvider, string name, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Check.NotNull(user, nameof(user));

        await UserRepository.EnsureCollectionLoadedAsync(user, u => u.Tokens, cancellationToken);

        return user.FindToken(loginProvider, name)?.Value;
    }

    public virtual Task SetAuthenticatorKeyAsync(IdentityUser user, string key, CancellationToken cancellationToken = default)
    {
        return SetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, key, cancellationToken);
    }

    /// <summary>
    /// Task<string?>
    /// </summary>
    public virtual Task<string?> GetAuthenticatorKeyAsync(IdentityUser user, CancellationToken cancellationToken = default)
    {
        return GetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, cancellationToken);
    }

    /// <summary>
    /// Returns how many recovery code are still valid for a user.
    /// </summary>
    /// <param name="user">The user who owns the recovery code.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The number of valid recovery codes for the user..</returns>
    /// <summary>
    /// Task<int>
    /// </summary>
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
    /// Updates the recovery codes for the user while invalidating any previous recovery codes.
    /// </summary>
    /// <param name="user">The user to store new recovery codes for.</param>
    /// <param name="recoveryCodes">The new recovery codes for the user.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The new recovery codes for the user.</returns>
    public virtual Task ReplaceCodesAsync(IdentityUser user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken = default)
    {
        var mergedCodes = string.Join(";", recoveryCodes);
        return SetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, mergedCodes, cancellationToken);
    }

    /// <summary>
    /// Returns whether a recovery code is valid for a user. Note: recovery codes are only valid
    /// once, and will be invalid after use.
    /// </summary>
    /// <param name="user">The user who owns the recovery code.</param>
    /// <param name="code">The recovery code to use.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>True if the recovery code was found for the user.</returns>
    /// <summary>
    /// Task<bool>
    /// </summary>
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
    /// Task<string>
    /// </summary>
    public virtual Task<string> GetInternalLoginProviderAsync()
    {
        return Task.FromResult(InternalLoginProvider);
    }

    /// <summary>
    /// Task<string>
    /// </summary>
    public virtual Task<string> GetAuthenticatorKeyTokenNameAsync()
    {
        return Task.FromResult(AuthenticatorKeyTokenName);
    }

    /// <summary>
    /// Task<string>
    /// </summary>
    public virtual Task<string> GetRecoveryCodeTokenNameAsync()
    {
        return Task.FromResult(RecoveryCodeTokenName);
    }

    public virtual void Dispose()
    {

    }
}
