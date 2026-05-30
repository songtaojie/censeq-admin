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

[Authorize]
public class ProfileAppService : IdentityAppServiceBase, IProfileAppService
{
    protected IdentityUserManager UserManager { get; }
    protected IOptions<IdentityOptions> IdentityOptions { get; }

    public ProfileAppService(
        IdentityUserManager userManager,
        IOptions<IdentityOptions> identityOptions)
    {
        UserManager = userManager;
        IdentityOptions = identityOptions;
    }

    public virtual async Task<ProfileDto> GetAsync()
    {
        var currentUser = await UserManager.GetByIdAsync(CurrentUser.GetId());

        var profile = ObjectMapper.Map<IdentityUser, ProfileDto>(currentUser);
        profile.SetProperty("signature", currentUser.GetProperty<string>("signature", null));

        return profile;
    }

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
