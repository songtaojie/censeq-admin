using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Censeq.Account.Settings;
using Volo.Abp.Auditing;
using Censeq.Identity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Settings;
using Volo.Abp.Validation;
using IdentityUser = Censeq.Identity.Entities.IdentityUser;

namespace Censeq.Account.Web.Pages.Account;

/// <summary>
/// 注册页面模型。
/// </summary>
public class RegisterModel : AccountPageModel
{

    /// <summary>
    /// 返回地址。
    /// </summary>
    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    /// <summary>
    /// 返回地址哈希。
    /// </summary>
    [BindProperty(SupportsGet = true)]
    public string? ReturnUrlHash { get; set; }

    /// <summary>
    /// 输入数据。
    /// </summary>
    [BindProperty]
    public PostInput Input { get; set; } = default!;

    /// <summary>
    /// 是否外部登录。
    /// </summary>
    [BindProperty(SupportsGet = true)]
    public bool IsExternalLogin { get; set; }

    /// <summary>
    /// 外部登录认证方案。
    /// </summary>
    [BindProperty(SupportsGet = true)]
    public string? ExternalLoginAuthSchema { get; set; }

    /// <summary>
    /// 外部登录提供者列表。
    /// </summary>
    public IEnumerable<ExternalProviderModel>? ExternalProviders { get; set; }
    /// <summary>
    /// 可见外部登录提供者列表。
    /// </summary>
    public IEnumerable<ExternalProviderModel>? VisibleExternalProviders => ExternalProviders?.Where(x => !string.IsNullOrWhiteSpace(x.DisplayName));
    /// <summary>
    /// 是否启用本地注册。
    /// </summary>
    public bool EnableLocalRegister { get; set; }
    /// <summary>
    /// 是否仅允许外部登录。
    /// </summary>
    public bool IsExternalLoginOnly => EnableLocalRegister == false && ExternalProviders?.Count() == 1;
    /// <summary>
    /// 外部登录方案。
    /// </summary>
    public string? ExternalLoginScheme => IsExternalLoginOnly ? ExternalProviders?.SingleOrDefault()?.AuthenticationScheme : null;

    /// <summary>
    /// 认证方案提供者。
    /// </summary>
    protected IAuthenticationSchemeProvider SchemeProvider { get; }

    /// <summary>
    /// 账户配置项。
    /// </summary>
    protected CenseqAccountOptions AccountOptions { get; }
    /// <summary>
    /// Identity 动态声明主体贡献器缓存。
    /// </summary>
    protected IdentityDynamicClaimsPrincipalContributorCache IdentityDynamicClaimsPrincipalContributorCache { get; }

    /// <summary>
    /// 初始化 RegisterModel 实例。
    /// </summary>
    /// <param name="accountAppService">账户应用服务。</param>
    /// <param name="schemeProvider">认证方案提供者。</param>
    /// <param name="accountOptions">账户配置项。</param>
    /// <param name="identityDynamicClaimsPrincipalContributorCache">Identity 动态声明主体贡献器缓存。</param>
    public RegisterModel(
        IAccountAppService accountAppService,
        IAuthenticationSchemeProvider schemeProvider,
        IOptions<CenseqAccountOptions> accountOptions,
        IdentityDynamicClaimsPrincipalContributorCache identityDynamicClaimsPrincipalContributorCache)
    {
        SchemeProvider = schemeProvider;
        IdentityDynamicClaimsPrincipalContributorCache = identityDynamicClaimsPrincipalContributorCache;
        //AccountAppService = accountAppService;
        AccountOptions = accountOptions.Value;
    }

    /// <summary>
    /// 异步处理页面 GET 请求。
    /// </summary>
    /// <returns>页面处理结果。</returns>
    public virtual async Task<IActionResult> OnGetAsync()
    {
        ExternalProviders = await GetExternalProviders();

        if (!await CheckSelfRegistrationAsync())
        {
            if (IsExternalLoginOnly)
            {
                return await OnPostExternalLogin(ExternalLoginScheme!);
            }

            Alerts.Warning(L["SelfRegistrationDisabledMessage"]);
        }

        await TrySetEmailAsync();

        return Page();
    }

    /// <summary>
    /// 异步尝试设置邮箱。
    /// </summary>
    /// <returns>表示异步操作的任务。</returns>
    protected virtual async Task TrySetEmailAsync()
    {
        if (IsExternalLogin)
        {
            var externalLoginInfo = await SignInManager.GetExternalLoginInfoAsync();
            if (externalLoginInfo == null)
            {
                return;
            }

            if (!externalLoginInfo.Principal.Identities.Any())
            {
                return;
            }

            var identity = externalLoginInfo.Principal.Identities.First();
            var emailClaim = identity.FindFirst(AbpClaimTypes.Email) ?? identity.FindFirst(ClaimTypes.Email);

            if (emailClaim == null)
            {
                return;
            }

            var userName = await UserManager.GetUserNameFromEmailAsync(emailClaim.Value);
            Input = new PostInput { UserName = userName, EmailAddress = emailClaim.Value };
        }
    }

    /// <summary>
    /// 异步处理页面 POST 请求。
    /// </summary>
    /// <returns>页面处理结果。</returns>
    public virtual async Task<IActionResult> OnPostAsync()
    {
        try
        {
            ExternalProviders = await GetExternalProviders();

            if (!await CheckSelfRegistrationAsync())
            {
                throw new UserFriendlyException(L["SelfRegistrationDisabledMessage"]);
            }

            if (IsExternalLogin)
            {
                var externalLoginInfo = await SignInManager.GetExternalLoginInfoAsync();
                if (externalLoginInfo == null)
                {
                    Logger.LogWarning("External login info is not available");
                    return RedirectToPage("./Login");
                }
                if (Input.UserName.IsNullOrWhiteSpace())
                {
                    if (string.IsNullOrWhiteSpace(Input.EmailAddress))
                    {
                        Logger.LogWarning("External login: EmailAddress is required");
                        return RedirectToPage("./Login");
                    }
                    Input.UserName = await UserManager.GetUserNameFromEmailAsync(Input.EmailAddress);
                }
                if (string.IsNullOrWhiteSpace(Input.EmailAddress))
                {
                    return RedirectToPage("./Login");
                }
                await RegisterExternalUserAsync(externalLoginInfo, Input.UserName!, Input.EmailAddress!);
            }
            else
            {
                await RegisterLocalUserAsync();
            }

            return Redirect(ReturnUrl ?? "~/"); //TODO: How to ensure safety? IdentityServer requires it however it should be checked somehow!
        }
        catch (BusinessException e)
        {
            Alerts.Danger(GetLocalizeExceptionMessage(e));
            return Page();
        }
    }

    /// <summary>
    /// 异步注册本地用户。
    /// </summary>
    /// <returns>表示异步操作的任务。</returns>
    protected virtual async Task RegisterLocalUserAsync()
    {
        ValidateModel();

        var userDto = await AccountAppService.RegisterAsync(
            new RegisterDto
            {
                AppName = "MVC",
                EmailAddress = Input.EmailAddress!,
                Password = Input.Password!,
                UserName = Input.UserName!
            }
        );

        var user = await UserManager.GetByIdAsync(userDto.Id);
        await SignInManager.SignInAsync(user, isPersistent: true);

        // Clear the dynamic claims cache.
        await IdentityDynamicClaimsPrincipalContributorCache.ClearAsync(user.Id, user.TenantId);
    }

    /// <summary>
    /// 异步注册外部用户。
    /// </summary>
    /// <param name="externalLoginInfo">外部登录信息。</param>
    /// <param name="userName">用户名。</param>
    /// <param name="emailAddress">邮箱地址。</param>
    /// <returns>表示异步操作的任务。</returns>
    protected virtual async Task RegisterExternalUserAsync(ExternalLoginInfo externalLoginInfo, string userName, string emailAddress)
    {
        await IdentityOptions.SetAsync();

        var user = new IdentityUser(GuidGenerator.Create(), userName, emailAddress, CurrentTenant.Id);

        (await UserManager.CreateAsync(user)).CheckErrors();
        (await UserManager.AddDefaultRolesAsync(user)).CheckErrors();

        var userLoginAlreadyExists = user.Logins.Any(x =>
            x.TenantId == user.TenantId &&
            x.LoginProvider == externalLoginInfo.LoginProvider &&
            x.ProviderKey == externalLoginInfo.ProviderKey);

        if (!userLoginAlreadyExists)
        {
            (await UserManager.AddLoginAsync(user, new UserLoginInfo(
                externalLoginInfo.LoginProvider,
                externalLoginInfo.ProviderKey,
                externalLoginInfo.ProviderDisplayName
            ))).CheckErrors();
        }

        await SignInManager.SignInAsync(user, isPersistent: true, ExternalLoginAuthSchema);

        // Clear the dynamic claims cache.
        await IdentityDynamicClaimsPrincipalContributorCache.ClearAsync(user.Id, user.TenantId);
    }

    /// <summary>
    /// 异步检查是否允许自助注册。
    /// </summary>
    /// <returns>异步操作结果。</returns>
    protected virtual async Task<bool> CheckSelfRegistrationAsync()
    {
        EnableLocalRegister = await SettingProvider.IsTrueAsync(AccountSettingNames.EnableLocalLogin) &&
                              await SettingProvider.IsTrueAsync(AccountSettingNames.IsSelfRegistrationEnabled);

        if (IsExternalLogin)
        {
            return true;
        }

        if (!EnableLocalRegister)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 获取外部登录提供者。
    /// </summary>
    /// <returns>外部登录提供者列表。</returns>
    protected virtual async Task<List<ExternalProviderModel>> GetExternalProviders()
    {
        var schemes = await SchemeProvider.GetAllSchemesAsync();

        return schemes
            .Where(x => x.DisplayName != null || x.Name.Equals(AccountOptions.WindowsAuthenticationSchemeName, StringComparison.OrdinalIgnoreCase))
            .Select(x => new ExternalProviderModel
            {
                DisplayName = x.DisplayName ?? string.Empty,
                AuthenticationScheme = x.Name
            })
            .ToList();
    }

    /// <summary>
    /// 处理外部登录提交。
    /// </summary>
    /// <param name="provider">提供者。</param>
    /// <returns>页面处理结果。</returns>
    protected virtual async Task<IActionResult> OnPostExternalLogin(string provider)
    {
        var redirectUrl = Url.Page("./Login", pageHandler: "ExternalLoginCallback", values: new { ReturnUrl, ReturnUrlHash });
        var properties = SignInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        properties.Items["scheme"] = provider;

        return await Task.FromResult(Challenge(properties, provider));
    }

    /// <summary>
    /// 提交输入模型。
    /// </summary>
    public class PostInput
    {
        /// <summary>
        /// 用户名。
        /// </summary>
        [Required]
        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxUserNameLength))]
        public string? UserName { get; set; }

        /// <summary>
        /// 邮箱地址。
        /// </summary>
        [Required]
        [EmailAddress]
        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxEmailLength))]
        public string? EmailAddress { get; set; }

        /// <summary>
        /// 密码。
        /// </summary>
        [Required]
        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPasswordLength))]
        [DataType(DataType.Password)]
        [DisableAuditing]
        public string? Password { get; set; }
    }

    /// <summary>
    /// 外部登录提供者模型。
    /// </summary>
    public class ExternalProviderModel
    {
        /// <summary>
        /// 显示名称。
        /// </summary>
        public required string DisplayName { get; set; }
        /// <summary>
        /// 认证方案。
        /// </summary>
        public string? AuthenticationScheme { get; set; }
    }
}
