using System;
using System.Linq;
using System.Threading.Tasks;
using Censeq.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;

namespace Censeq.Identity;

/// <summary>
/// 外部登录提供程序基类
/// </summary>
public abstract class ExternalLoginProviderBase : IExternalLoginProvider
{
    /// <summary>
    /// IGuidGenerator
    /// </summary>
    protected IGuidGenerator GuidGenerator { get; }
    /// <summary>
    /// ICurrent租户
    /// </summary>
    protected ICurrentTenant CurrentTenant { get; }
    /// <summary>
    /// 身份用户管理器
    /// </summary>
    protected IdentityUserManager UserManager { get; }
    /// <summary>
    /// I身份用户仓储
    /// </summary>
    protected IIdentityUserRepository IdentityUserRepository { get; }
    /// <summary>
    /// IOptions<IdentityOptions>
    /// </summary>
    protected IOptions<IdentityOptions> IdentityOptions { get; }
    protected ExternalLoginProviderBase(
        IGuidGenerator guidGenerator,
        ICurrentTenant currentTenant,
        IdentityUserManager userManager,
        IIdentityUserRepository identityUserRepository,
        IOptions<IdentityOptions> identityOptions)
    {
        GuidGenerator = guidGenerator;
        CurrentTenant = currentTenant;
        UserManager = userManager;
        IdentityUserRepository = identityUserRepository;
        IdentityOptions = identityOptions;
    }

    /// <summary>
    /// Task<bool>
    /// </summary>
    public abstract Task<bool> TryAuthenticateAsync(string userName, string plainPassword);

    /// <summary>
    /// Task<bool>
    /// </summary>
    public abstract Task<bool> IsEnabledAsync();

    /// <summary>
    /// Task<IdentityUser>
    /// </summary>
    public virtual async Task<IdentityUser> CreateUserAsync(string userName, string providerName)
    {
        await IdentityOptions.SetAsync();

        var externalUser = await GetUserInfoAsync(userName);

        return await CreateUserAsync(externalUser, userName, providerName);
    }
    
    /// <summary>
    /// Task<IdentityUser>
    /// </summary>
    protected virtual async Task<IdentityUser> CreateUserAsync(ExternalLoginUserInfo externalUser, string userName, string providerName)
    {
        NormalizeExternalLoginUserInfo(externalUser, userName);
        
        var user = new IdentityUser(
            GuidGenerator.Create(),
            userName,
            externalUser.Email,
            tenantId: CurrentTenant.Id
        );

        user.Name = externalUser.Name;
        user.Surname = externalUser.Surname;

        user.IsExternal = true;

        user.SetEmailConfirmed(externalUser.EmailConfirmed ?? false);
        user.SetPhoneNumber(externalUser.PhoneNumber, externalUser.PhoneNumberConfirmed ?? false);

        (await UserManager.CreateAsync(user)).CheckErrors();

        if (externalUser.TwoFactorEnabled != null)
        {
            (await UserManager.SetTwoFactorEnabledAsync(user, externalUser.TwoFactorEnabled.Value)).CheckErrors();
        }

        (await UserManager.AddDefaultRolesAsync(user)).CheckErrors();
        (await UserManager.AddLoginAsync(
                    user,
                    new UserLoginInfo(
                        providerName,
                        externalUser.ProviderKey,
                        providerName
                    )
                )
            ).CheckErrors();

        return user;
    }

    public virtual async Task UpdateUserAsync(IdentityUser user, string providerName)
    {
        await IdentityOptions.SetAsync();
        
        var externalUser = await GetUserInfoAsync(user);

        await UpdateUserAsync(user, externalUser, providerName);
    }
    
    protected virtual async Task UpdateUserAsync(IdentityUser user, ExternalLoginUserInfo externalUser ,string providerName)
    {
        NormalizeExternalLoginUserInfo(externalUser, user.UserName);

        if (!externalUser.Name.IsNullOrWhiteSpace())
        {
            user.Name = externalUser.Name;
        }

        if (!externalUser.Surname.IsNullOrWhiteSpace())
        {
            user.Surname = externalUser.Surname;
        }

        if (user.PhoneNumber != externalUser.PhoneNumber)
        {
            if (!externalUser.PhoneNumber.IsNullOrWhiteSpace())
            {
                await UserManager.SetPhoneNumberAsync(user, externalUser.PhoneNumber);
                user.SetPhoneNumberConfirmed(externalUser.PhoneNumberConfirmed == true);
            }
        }
        else
        {
            if (!user.PhoneNumber.IsNullOrWhiteSpace() &&
                user.PhoneNumberConfirmed == false &&
                externalUser.PhoneNumberConfirmed == true)
            {
                user.SetPhoneNumberConfirmed(true);
            }
        }

        if (!string.Equals(user.Email, externalUser.Email, StringComparison.OrdinalIgnoreCase))
        {
            (await UserManager.SetEmailAsync(user, externalUser.Email)).CheckErrors();
            user.SetEmailConfirmed(externalUser.EmailConfirmed ?? false);
        }

        if (externalUser.TwoFactorEnabled != null)
        {
            (await UserManager.SetTwoFactorEnabledAsync(user, externalUser.TwoFactorEnabled.Value)).CheckErrors();
        }

        await IdentityUserRepository.EnsureCollectionLoadedAsync(user, u => u.Logins);

        var userLogin = user.Logins.FirstOrDefault(l => l.LoginProvider == providerName);
        if (userLogin != null)
        {
            if (userLogin.ProviderKey != externalUser.ProviderKey)
            {
                (await UserManager.RemoveLoginAsync(user, providerName, userLogin.ProviderKey)).CheckErrors();
                (await UserManager.AddLoginAsync(user, new UserLoginInfo(providerName, externalUser.ProviderKey, providerName))).CheckErrors();
            }
        }
        else
        {
            (await UserManager.AddLoginAsync(user, new UserLoginInfo(providerName, externalUser.ProviderKey, providerName))).CheckErrors();
        }

        user.IsExternal = true;

        (await UserManager.UpdateAsync(user)).CheckErrors();
    }

    /// <summary>
    /// Task<External登录用户Info>
    /// </summary>
    protected abstract Task<ExternalLoginUserInfo> GetUserInfoAsync(string userName);

    /// <summary>
    /// Task<External登录用户Info>
    /// </summary>
    protected virtual Task<ExternalLoginUserInfo> GetUserInfoAsync(IdentityUser user)
    {
        return GetUserInfoAsync(user.UserName);
    }

    private static void NormalizeExternalLoginUserInfo(
        ExternalLoginUserInfo externalUser,
        string userName
    )
    {
        if (externalUser.ProviderKey.IsNullOrWhiteSpace())
        {
            externalUser.ProviderKey = userName;
        }
    }
}
