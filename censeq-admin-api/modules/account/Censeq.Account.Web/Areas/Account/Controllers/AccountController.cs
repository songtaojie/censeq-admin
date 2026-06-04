using Asp.Versioning;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Censeq.Account.Localization;
using Censeq.Account.Settings;
using Censeq.Account.Web.Areas.Account.Controllers.Models;
using Volo.Abp.AspNetCore.Mvc;
using Censeq.Identity;
using Volo.Abp.Settings;
using Volo.Abp.Validation;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using UserLoginInfo = Censeq.Account.Web.Areas.Account.Controllers.Models.UserLoginInfo;
using IdentityUser = Censeq.Identity.Entities.IdentityUser;
using Censeq.Identity.AspNetCore;

namespace Censeq.Account.Web.Areas.Account.Controllers;

/// <summary>
/// 账户控制器，提供对应的 HTTP API。
/// </summary>
[RemoteService(Name = AccountRemoteServiceConsts.RemoteServiceName)]
[Controller]
[ControllerName("Login")]
[Area("account")]
[Route("api/account")]
public class AccountController : AbpControllerBase
{
    /// <summary>
    /// 登录管理器。
    /// </summary>
    protected SignInManager<IdentityUser> SignInManager { get; }
    /// <summary>
    /// 用户管理器。
    /// </summary>
    protected IdentityUserManager UserManager { get; }
    /// <summary>
    /// 设置提供者。
    /// </summary>
    protected ISettingProvider SettingProvider { get; }
    /// <summary>
    /// Identity 安全日志管理器。
    /// </summary>
    protected IdentitySecurityLogManager IdentitySecurityLogManager { get; }
    /// <summary>
    /// Identity 配置项。
    /// </summary>
    protected IOptions<IdentityOptions> IdentityOptions { get; }
    /// <summary>
    /// Identity 动态声明主体贡献器缓存。
    /// </summary>
    protected IdentityDynamicClaimsPrincipalContributorCache IdentityDynamicClaimsPrincipalContributorCache { get; }

    /// <summary>
    /// 初始化 AccountController 实例。
    /// </summary>
    /// <param name="signInManager">登录管理器。</param>
    /// <param name="userManager">用户管理器。</param>
    /// <param name="settingProvider">设置提供者。</param>
    /// <param name="identitySecurityLogManager">Identity 安全日志管理器。</param>
    /// <param name="identityOptions">Identity 配置项。</param>
    /// <param name="identityDynamicClaimsPrincipalContributorCache">Identity 动态声明主体贡献器缓存。</param>
    public AccountController(
        SignInManager<IdentityUser> signInManager,
        IdentityUserManager userManager,
        ISettingProvider settingProvider,
        IdentitySecurityLogManager identitySecurityLogManager,
        IOptions<IdentityOptions> identityOptions,
        IdentityDynamicClaimsPrincipalContributorCache identityDynamicClaimsPrincipalContributorCache)
    {
        LocalizationResource = typeof(AccountResource);

        SignInManager = signInManager;
        UserManager = userManager;
        SettingProvider = settingProvider;
        IdentitySecurityLogManager = identitySecurityLogManager;
        IdentityOptions = identityOptions;
        IdentityDynamicClaimsPrincipalContributorCache = identityDynamicClaimsPrincipalContributorCache;
    }

    /// <summary>
    /// 执行登录。
    /// </summary>
    /// <param name="login">登录。</param>
    /// <returns>异步操作结果。</returns>
    [HttpPost]
    [Route("login")]
    public virtual async Task<AbpLoginResult> Login(UserLoginInfo login)
    {
        await CheckLocalLoginAsync();

        ValidateLoginInfo(login);

        await ReplaceEmailToUsernameOfInputIfNeeds(login);

        await IdentityOptions.SetAsync();

        var signInResult = await SignInManager.PasswordSignInAsync(
            login.UserNameOrEmailAddress!,
            login.Password!,
            login.RememberMe,
            true
        );

        await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
        {
            Identity = IdentitySecurityLogIdentityConsts.Identity,
            Action = signInResult.ToIdentitySecurityLogAction(),
            UserName = login.UserNameOrEmailAddress!
        });

        if (signInResult.Succeeded)
        {
            var user = await UserManager.FindByNameAsync(login.UserNameOrEmailAddress!);
            if (user != null)
            {
                // Clear the dynamic claims cache.
                await IdentityDynamicClaimsPrincipalContributorCache.ClearAsync(user.Id, user.TenantId);
            }
        }

        return GetAbpLoginResult(signInResult);
    }

    /// <summary>
    /// 登出。
    /// </summary>
    /// <returns>表示异步操作的任务。</returns>
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

    /// <summary>
    /// Check 密码 Compatible。
    /// </summary>
    /// <param name="login">登录。</param>
    /// <returns>异步操作结果。</returns>
    [HttpPost]
    [Route("checkPassword")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public virtual Task<AbpLoginResult> CheckPasswordCompatible(UserLoginInfo login)
    {
        return CheckPassword(login);
    }

    /// <summary>
    /// Check 密码。
    /// </summary>
    /// <param name="login">登录。</param>
    /// <returns>异步操作结果。</returns>
    [HttpPost]
    [Route("check-password")]
    public virtual async Task<AbpLoginResult> CheckPassword(UserLoginInfo login)
    {
        ValidateLoginInfo(login);

        await ReplaceEmailToUsernameOfInputIfNeeds(login);

        var identityUser = await UserManager.FindByNameAsync(login.UserNameOrEmailAddress!);

        if (identityUser == null)
        {
            return new AbpLoginResult(LoginResultType.InvalidUserNameOrPassword);
        }

        await IdentityOptions.SetAsync();
        return GetAbpLoginResult(await SignInManager.CheckPasswordSignInAsync(identityUser, login.Password!, true));
    }

    /// <summary>
    /// 必要时将输入邮箱替换为用户名。
    /// </summary>
    /// <param name="login">登录。</param>
    /// <returns>表示异步操作的任务。</returns>
    protected virtual async Task ReplaceEmailToUsernameOfInputIfNeeds(UserLoginInfo login)
    {
        if (!ValidationHelper.IsValidEmailAddress(login.UserNameOrEmailAddress!))
        {
            return;
        }

        var userByUsername = await UserManager.FindByNameAsync(login.UserNameOrEmailAddress!);
        if (userByUsername != null)
        {
            return;
        }

        var userByEmail = await UserManager.FindByEmailAsync(login.UserNameOrEmailAddress!);
        if (userByEmail == null)
        {
            return;
        }

        login.UserNameOrEmailAddress = userByEmail.UserName;
    }

    /// <summary>
    /// Get Abp 登录 结果。
    /// </summary>
    /// <param name="result">结果。</param>
    /// <returns>返回结果。</returns>
    private static AbpLoginResult GetAbpLoginResult(SignInResult result)
    {
        if (result.IsLockedOut)
        {
            return new AbpLoginResult(LoginResultType.LockedOut);
        }

        if (result.RequiresTwoFactor)
        {
            return new AbpLoginResult(LoginResultType.RequiresTwoFactor);
        }

        if (result.IsNotAllowed)
        {
            return new AbpLoginResult(LoginResultType.NotAllowed);
        }

        if (!result.Succeeded)
        {
            return new AbpLoginResult(LoginResultType.InvalidUserNameOrPassword);
        }

        return new AbpLoginResult(LoginResultType.Success);
    }

    /// <summary>
    /// 验证登录信息。
    /// </summary>
    /// <param name="login">登录。</param>
    protected virtual void ValidateLoginInfo(UserLoginInfo login)
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

    /// <summary>
    /// 异步检查本地登录。
    /// </summary>
    /// <returns>表示异步操作的任务。</returns>
    protected virtual async Task CheckLocalLoginAsync()
    {
        if (!await SettingProvider.IsTrueAsync(AccountSettingNames.EnableLocalLogin))
        {
            throw new UserFriendlyException(L["LocalLoginDisabledMessage"]);
        }
    }
}
