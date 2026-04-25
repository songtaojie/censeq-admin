using System;
using System.Threading.Tasks;
using Censeq.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;

namespace Censeq.Identity;

/// <summary>
/// 外部登录提供程序With密码基类
/// </summary>
public abstract class ExternalLoginProviderWithPasswordBase : ExternalLoginProviderBase, IExternalLoginProviderWithPassword
{
    public bool CanObtainUserInfoWithoutPassword { get; }
    
    public ExternalLoginProviderWithPasswordBase(
        IGuidGenerator guidGenerator, 
        ICurrentTenant currentTenant,
        IdentityUserManager userManager,
        IIdentityUserRepository identityUserRepository,
        IOptions<IdentityOptions> identityOptions,
        bool canObtainUserInfoWithoutPassword = false) : 
        base(guidGenerator, 
            currentTenant, 
            userManager,
            identityUserRepository,
            identityOptions)
    {
        CanObtainUserInfoWithoutPassword = canObtainUserInfoWithoutPassword;
    }
    
    /// <summary>
    /// Task<IdentityUser>
    /// </summary>
    public async Task<IdentityUser> CreateUserAsync(string userName, string providerName, string plainPassword)
    {
        if (CanObtainUserInfoWithoutPassword)
        {
            return await CreateUserAsync(userName, providerName);
        }
        
        await IdentityOptions.SetAsync();

        var externalUser = await GetUserInfoAsync(userName, plainPassword);

        return await CreateUserAsync(externalUser, userName, providerName);
    }

    public async Task UpdateUserAsync(IdentityUser user, string providerName, string plainPassword)
    {
        if (CanObtainUserInfoWithoutPassword)
        {
            await UpdateUserAsync(user, providerName);
            return;
        }
        
        await IdentityOptions.SetAsync();
        
        var externalUser = await GetUserInfoAsync(user, plainPassword);

        await UpdateUserAsync(user, externalUser, providerName);
    }

    /// <summary>
    /// Task<External登录用户Info>
    /// </summary>
    protected override Task<ExternalLoginUserInfo> GetUserInfoAsync(string userName)
    {
        throw new NotImplementedException($"{nameof(GetUserInfoAsync)} is not implemented default. It should be overriden and implemented by the deriving class!");
    }

    /// <summary>
    /// Task<External登录用户Info>
    /// </summary>
    protected abstract Task<ExternalLoginUserInfo> GetUserInfoAsync(string userName, string plainPassword);
    
    /// <summary>
    /// Task<External登录用户Info>
    /// </summary>
    protected virtual Task<ExternalLoginUserInfo> GetUserInfoAsync(IdentityUser user, string plainPassword)
    {
        return GetUserInfoAsync(user.UserName, plainPassword);
    }
}