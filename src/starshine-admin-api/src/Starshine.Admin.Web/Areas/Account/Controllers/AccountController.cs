using System;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Starshine.Admin.Web.Areas.Account.Controllers.Models;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Identity;
using Volo.Abp.Settings;
using Volo.Abp.Validation;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using UserLoginInput = Starshine.Admin.Web.Areas.Account.Controllers.Models.UserLoginInput;
using IdentityUser = Volo.Abp.Identity.IdentityUser;
using Starshine.Admin.Consts;
using Volo.Abp;
using Starshine.Admin.Localization;
using Starshine.Admin.Settings;
using Starshine.Abp.Identity.AspNetCore;

namespace Starshine.Admin.Web.Areas.Account.Controllers;

[RemoteService(Name = AccountRemoteServiceConsts.RemoteServiceName)]
[Controller]
[ControllerName("Login")]
[Area("account")]
[Route("api/account")]
public class AccountController : AbpControllerBase
{
    protected SignInManager<IdentityUser> SignInManager { get; }
    protected IdentityUserManager UserManager { get; }
    protected ISettingProvider SettingProvider { get; }
    protected IdentitySecurityLogManager IdentitySecurityLogManager { get; }
    protected IOptions<IdentityOptions> IdentityOptions { get; }
    protected IdentityDynamicClaimsPrincipalContributorCache IdentityDynamicClaimsPrincipalContributorCache { get; }

    public AccountController(
        SignInManager<IdentityUser> signInManager,
        IdentityUserManager userManager,
        ISettingProvider settingProvider,
        IdentitySecurityLogManager identitySecurityLogManager,
        IOptions<IdentityOptions> identityOptions,
        IdentityDynamicClaimsPrincipalContributorCache identityDynamicClaimsPrincipalContributorCache)
    {
        LocalizationResource = typeof(AdminResource);

        SignInManager = signInManager;
        UserManager = userManager;
        SettingProvider = settingProvider;
        IdentitySecurityLogManager = identitySecurityLogManager;
        IdentityOptions = identityOptions;
        IdentityDynamicClaimsPrincipalContributorCache = identityDynamicClaimsPrincipalContributorCache;
    }

    [HttpPost]
    [Route("login")]
    public virtual async Task<UserLoginOutput> Login(UserLoginInput login)
    {
        await CheckLocalLoginAsync();

        ValidateLoginInfo(login);

        await ReplaceEmailToUsernameOfInputIfNeeds(login);

        await IdentityOptions.SetAsync();

        var signInResult = await SignInManager.PasswordSignInAsync(
            login.UserNameOrEmailAddress,
            login.Password,
            login.RememberMe,
            true
        );

        await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
        {
            Identity = IdentitySecurityLogIdentityConsts.Identity,
            Action = signInResult.ToIdentitySecurityLogAction(),
            UserName = login.UserNameOrEmailAddress
        });

        if (signInResult.Succeeded)
        {
            var user = await UserManager.FindByNameAsync(login.UserNameOrEmailAddress);
            if (user != null)
            {
                // Clear the dynamic claims cache.
                await IdentityDynamicClaimsPrincipalContributorCache.ClearAsync(user.Id, user.TenantId);
            }
        }

        return GetUserLoginResult(signInResult);
    }

    [HttpGet]
    [Route("logout")]
    public virtual async Task Logout()
    {
        await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
        {
            Identity = IdentitySecurityLogIdentityConsts.Identity,
            Action = IdentitySecurityLogActionConsts.Logout
        });

        await SignInManager.SignOutAsync();
    }

    [HttpPost]
    [Route("checkPassword")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public virtual Task<UserLoginOutput> CheckPasswordCompatible(UserLoginInput login)
    {
        return CheckPassword(login);
    }

    [HttpPost]
    [Route("check-password")]
    public virtual async Task<UserLoginOutput> CheckPassword(UserLoginInput login)
    {
        ValidateLoginInfo(login);

        await ReplaceEmailToUsernameOfInputIfNeeds(login);

        var identityUser = await UserManager.FindByNameAsync(login.UserNameOrEmailAddress);

        if (identityUser == null)
        {
            return new UserLoginOutput(UserLoginType.InvalidUserNameOrPassword);
        }

        await IdentityOptions.SetAsync();
        return GetUserLoginResult(await SignInManager.CheckPasswordSignInAsync(identityUser, login.Password, true));
    }

    protected virtual async Task ReplaceEmailToUsernameOfInputIfNeeds(UserLoginInput login)
    {
        if (!ValidationHelper.IsValidEmailAddress(login.UserNameOrEmailAddress))
        {
            return;
        }

        var userByUsername = await UserManager.FindByNameAsync(login.UserNameOrEmailAddress);
        if (userByUsername != null)
        {
            return;
        }

        var userByEmail = await UserManager.FindByEmailAsync(login.UserNameOrEmailAddress);
        if (userByEmail == null)
        {
            return;
        }

        login.UserNameOrEmailAddress = userByEmail.UserName;
    }

    private static UserLoginOutput GetUserLoginResult(SignInResult result)
    {
        if (result.IsLockedOut)
        {
            return new UserLoginOutput(UserLoginType.LockedOut);
        }

        if (result.RequiresTwoFactor)
        {
            return new UserLoginOutput(UserLoginType.RequiresTwoFactor);
        }

        if (result.IsNotAllowed)
        {
            return new UserLoginOutput(UserLoginType.NotAllowed);
        }

        if (!result.Succeeded)
        {
            return new UserLoginOutput(UserLoginType.InvalidUserNameOrPassword);
        }

        return new UserLoginOutput(UserLoginType.Success);
    }

    protected virtual void ValidateLoginInfo(UserLoginInput login)
    {
        if (login == null)
        {
            throw new ArgumentException(nameof(login));
        }

        if (login.UserNameOrEmailAddress.IsNullOrEmpty())
        {
            throw new ArgumentNullException(nameof(login.UserNameOrEmailAddress));
        }

        if (login.Password.IsNullOrEmpty())
        {
            throw new ArgumentNullException(nameof(login.Password));
        }
    }

    protected virtual async Task CheckLocalLoginAsync()
    {
        if (!await SettingProvider.IsTrueAsync(AdminSettingNames.EnableLocalLogin))
        {
            throw new UserFriendlyException(L["LocalLoginDisabledMessage"]);
        }
    }
}
