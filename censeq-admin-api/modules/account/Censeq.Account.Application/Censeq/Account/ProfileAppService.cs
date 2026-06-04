using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Settings;
using Volo.Abp.Users;
using Censeq.Identity;
using Censeq.Identity.Settings;
using Censeq.Identity.Entities;

namespace Censeq.Account;

/// <summary>
/// 个人资料应用服务，提供个人资料查询和维护能力。
/// </summary>
[Authorize]
public class ProfileAppService : IdentityAppServiceBase, IProfileAppService
{
    /// <summary>
    /// 用户管理器。
    /// </summary>
    protected IdentityUserManager UserManager { get; }
    /// <summary>
    /// Identity 配置项。
    /// </summary>
    protected IOptions<IdentityOptions> IdentityOptions { get; }

    /// <summary>
    /// 初始化 ProfileAppService 实例。
    /// </summary>
    /// <param name="userManager">用户管理器。</param>
    /// <param name="identityOptions">Identity 配置项。</param>
    public ProfileAppService(
        IdentityUserManager userManager,
        IOptions<IdentityOptions> identityOptions)
    {
        UserManager = userManager;
        IdentityOptions = identityOptions;
    }

    /// <summary>
    /// 异步获取个人资料。
    /// </summary>
    /// <returns>个人资料。</returns>
    public virtual async Task<ProfileDto> GetAsync()
    {
        var currentUser = await UserManager.GetByIdAsync(CurrentUser.GetId());

        var profile = ObjectMapper.Map<IdentityUser, ProfileDto>(currentUser);
        profile.SetProperty("signature", currentUser.GetProperty<string>("signature", null));

        return profile;
    }

    /// <summary>
    /// 异步更新个人资料。
    /// </summary>
    /// <param name="input">输入数据。</param>
    /// <returns>个人资料。</returns>
    public virtual async Task<ProfileDto> UpdateAsync(UpdateProfileDto input)
    {
        await IdentityOptions.SetAsync();

        var user = await UserManager.GetByIdAsync(CurrentUser.GetId());

        if (input.ConcurrencyStamp != null)
        {
            user.ConcurrencyStamp = input.ConcurrencyStamp;
        }

        if (!string.Equals(user.UserName, input.UserName, StringComparison.InvariantCultureIgnoreCase))
        {
            if (await SettingProvider.IsTrueAsync(IdentitySettingNames.User.IsUserNameUpdateEnabled))
            {
                (await UserManager.SetUserNameAsync(user, input.UserName!)).CheckErrors();
            }
        }

        if (!string.Equals(user.Email, input.Email, StringComparison.InvariantCultureIgnoreCase))
        {
            if (await SettingProvider.IsTrueAsync(IdentitySettingNames.User.IsEmailUpdateEnabled))
            {
                (await UserManager.SetEmailAsync(user, input.Email!)).CheckErrors();
            }
        }

        if (user.PhoneNumber.IsNullOrWhiteSpace() && input.PhoneNumber.IsNullOrWhiteSpace())
        {
            input.PhoneNumber = user.PhoneNumber;
        }

        if (!string.Equals(user.PhoneNumber, input.PhoneNumber, StringComparison.InvariantCultureIgnoreCase))
        {
            (await UserManager.SetPhoneNumberAsync(user, input.PhoneNumber!)).CheckErrors();
        }

        user.Name = input.Name?.Trim();
        user.Surname = input.Surname?.Trim();
        user.SetProperty("AvatarUrl", input.AvatarUrl?.Trim());

        input.MapExtraPropertiesTo(user);
        SyncSignatureExtraProperty(input, user);

        (await UserManager.UpdateAsync(user)).CheckErrors();

        await CurrentUnitOfWork!.SaveChangesAsync();

        var profile = ObjectMapper.Map<IdentityUser, ProfileDto>(user);
        profile.SetProperty("signature", user.GetProperty<string>("signature", null));

        return profile;
    }

    /// <summary>
    /// 同步签名扩展属性。
    /// </summary>
    /// <param name="input">输入数据。</param>
    /// <param name="user">用户。</param>
    private static void SyncSignatureExtraProperty(UpdateProfileDto input, IdentityUser user)
    {
        if (!input.ExtraProperties.TryGetValue("signature", out var signatureValue))
        {
            return;
        }

        var signature = signatureValue?.ToString()?.Trim();
        if (signature.IsNullOrWhiteSpace())
        {
            user.ExtraProperties.Remove("signature");
            return;
        }

        user.SetProperty("signature", signature);
    }

    /// <summary>
    /// 异步修改密码。
    /// </summary>
    /// <param name="input">输入数据。</param>
    /// <returns>表示异步操作的任务。</returns>
    public virtual async Task ChangePasswordAsync(ChangePasswordInput input)
    {
        await IdentityOptions.SetAsync();

        var currentUser = await UserManager.GetByIdAsync(CurrentUser.GetId());

        if (currentUser.IsExternal)
        {
            throw new BusinessException(code: IdentityErrorCodes.ExternalUserPasswordChange);
        }

        if (currentUser.PasswordHash == null)
        {
            (await UserManager.AddPasswordAsync(currentUser, input.NewPassword)).CheckErrors();

            return;
        }

        (await UserManager.ChangePasswordAsync(currentUser, input.CurrentPassword, input.NewPassword)).CheckErrors();
    }
}
