using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using Volo.Abp.Caching;
using Censeq.Account.Settings;
using Volo.Abp.Auditing;
using Censeq.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;
using Volo.Abp.Settings;
using Volo.Abp.Validation;
using IdentityUser = Censeq.Identity.Entities.IdentityUser;
using Censeq.Account.Web.Settings;
using Censeq.Identity.AspNetCore;
using Censeq.TenantManagement;
using Censeq.Identity.Entities;
using Censeq.TenantManagement.Entities;
using Volo.Abp.Data;
using Lazy.Captcha.Core;

namespace Censeq.Account.Web.Pages.Account;

/// <summary>
/// 登录页面模型。
/// </summary>
public class LoginModel : AccountPageModel
{
    /// <summary>
    /// 返回地址。
    /// </summary>
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    /// <summary>
    /// 返回地址哈希。
    /// </summary>
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string? ReturnUrlHash { get; set; }

    /// <summary>
    /// 登录输入。
    /// </summary>
    [BindProperty]
    public LoginInputModel LoginInput { get; set; } = default!;

    /// <summary>
    /// 是否使用指定租户。
    /// </summary>
    [BindProperty]
    [Display(Name = "指定企业编码")]
    public bool UseSpecifiedTenant { get; set; }

    /// <summary>
    /// 企业租户编码。
    /// </summary>
    [BindProperty]
    public string? EnterpriseTenantCode { get; set; }

    /// <summary>授权链接带 __tenant 时为 true，租户编码只读并固定使用链接值。</summary>
    public bool IsEnterpriseTenantPresetFromLink { get; set; }

    /// <summary>
    /// 是否启用本地登录。
    /// </summary>
    public bool EnableLocalLogin { get; set; }

    /// <summary>
    /// 是否启用记住登录。
    /// </summary>
    public bool EnableRememberMe { get; set; }

    /// <summary>
    /// 是否启用验证码。
    /// </summary>
    public bool EnableCaptcha { get; set; }

    /// <summary>
    /// 是否启用自助注册。
    /// </summary>
    public bool IsSelfRegistrationEnabled { get; set; }

    //TODO: Why there is an ExternalProviders if only the VisibleExternalProviders is used.
    /// <summary>
    /// 外部登录提供者列表。
    /// </summary>
    public IEnumerable<ExternalProviderModel>? ExternalProviders { get; set; }
    /// <summary>
    /// 可见外部登录提供者列表。
    /// </summary>
    public IEnumerable<ExternalProviderModel>? VisibleExternalProviders => ExternalProviders?.Where(x => !string.IsNullOrWhiteSpace(x.DisplayName));

    /// <summary>
    /// 是否仅允许外部登录。
    /// </summary>
    public bool IsExternalLoginOnly => EnableLocalLogin == false && ExternalProviders?.Count() == 1;
    /// <summary>
    /// 外部登录方案。
    /// </summary>
    public string? ExternalLoginScheme => IsExternalLoginOnly ? ExternalProviders?.SingleOrDefault()?.AuthenticationScheme : null;

    //Optional IdentityServer services
    //public IIdentityServerInteractionService Interaction { get; set; }
    //public IClientStore ClientStore { get; set; }
    //public IEventService IdentityServerEvents { get; set; }

    /// <summary>
    /// 认证方案提供者。
    /// </summary>
    protected IAuthenticationSchemeProvider SchemeProvider { get; }
    /// <summary>
    /// 账户配置项。
    /// </summary>
    protected CenseqAccountOptions AccountOptions { get; }
    //protected IOptions<IdentityOptions> IdentityOptions { get; }
    /// <summary>
    /// Identity 动态声明主体贡献器缓存。
    /// </summary>
    protected IdentityDynamicClaimsPrincipalContributorCache IdentityDynamicClaimsPrincipalContributorCache { get; }
    /// <summary>
    /// Web 主机环境。
    /// </summary>
    protected IWebHostEnvironment WebHostEnvironment { get; }
    /// <summary>
    /// Identity 会话管理器。
    /// </summary>
    protected IdentitySessionManager IdentitySessionManager { get; }
    /// <summary>
    /// 是否显示取消按钮。
    /// </summary>
    public bool ShowCancelButton { get; set; }
    /// <summary>
    /// 是否显示迁移种子数据提示。
    /// </summary>
    public bool ShowRequireMigrateSeedMessage { get; set; }
    /// <summary>
    /// 登录错误消息。
    /// </summary>
    public string? LoginErrorMessage { get; set; }

    /// <summary>
    /// 初始化 LoginModel 实例。
    /// </summary>
    /// <param name="schemeProvider">认证方案提供者。</param>
    /// <param name="accountOptions">账户配置项。</param>
    /// <param name="identityDynamicClaimsPrincipalContributorCache">Identity 动态声明主体贡献器缓存。</param>
    /// <param name="webHostEnvironment">Web 主机环境。</param>
    /// <param name="identitySessionManager">Identity 会话管理器。</param>
    public LoginModel(
        IAuthenticationSchemeProvider schemeProvider,
        IOptions<CenseqAccountOptions> accountOptions,
        IdentityDynamicClaimsPrincipalContributorCache identityDynamicClaimsPrincipalContributorCache,
        IWebHostEnvironment webHostEnvironment,
        IdentitySessionManager identitySessionManager)
    {
        SchemeProvider = schemeProvider;
        AccountOptions = accountOptions.Value;
        IdentityDynamicClaimsPrincipalContributorCache = identityDynamicClaimsPrincipalContributorCache;
        WebHostEnvironment = webHostEnvironment;
        IdentitySessionManager = identitySessionManager;
    }

    /// <summary>
    /// 异步处理页面 GET 请求。
    /// </summary>
    /// <returns>页面处理结果。</returns>
    public virtual async Task<IActionResult> OnGetAsync()
    {
        LoginInput = new LoginInputModel();
        await InitializeTenantSelectionAsync();

        await ReloadLoginPageStateAsync();

        if (IsExternalLoginOnly)
        {
            return await OnPostExternalLogin(ExternalProviders!.First().AuthenticationScheme ?? string.Empty);
        }

        return Page();
    }

    /// <summary>
    /// 异步获取验证码。
    /// </summary>
    /// <returns>验证码结果。</returns>
    public virtual async Task<IActionResult> OnGetCaptchaAsync()
    {
        if (!await IsCaptchaEnabledAsync())
        {
            return new JsonResult(new
            {
                enabled = false
            });
        }

        var captchaId = Guid.NewGuid().ToString("N");
        var captcha = LazyServiceProvider.LazyGetRequiredService<ICaptcha>().Generate(captchaId);
        var captchaOptions = LazyServiceProvider.LazyGetRequiredService<IOptions<CaptchaOptions>>().Value;

        return new JsonResult(new
        {
            enabled = true,
            id = captchaId,
            img = captcha.Base64,
            expirySeconds = captchaOptions.ExpirySeconds
        });
    }

    /// <summary>
    /// 异步处理页面 POST 请求。
    /// </summary>
    /// <param name="action">action。</param>
    /// <returns>页面处理结果。</returns>
    public virtual async Task<IActionResult> OnPostAsync(string action)
    {
        if (string.Equals(action, "Login", StringComparison.OrdinalIgnoreCase))
        {
            if (!await TryPrepareTenantSelectionAsync())
            {
                await ReloadLoginPageStateAsync();
                return Page();
            }
        }

        await CheckLocalLoginAsync();

        //ValidateModel();
        ModelValidator?.Validate(ModelState);

        await ReloadLoginPageStateAsync();

        if (!await ValidateCaptchaAsync())
        {
            return Page();
        }

        var resolution = await ResolveLoginUserAsync();
        if (resolution.RedirectResult != null)
        {
            return resolution.RedirectResult;
        }

        var resolvedUser = resolution.User;
        if (resolvedUser == null)
        {
            await ReloadLoginPageStateAsync();
            return Page();
        }

        await IdentityOptions.SetAsync();

        var result = await SignInManager.PasswordSignInAsync(
            LoginInput.UserNameOrEmailAddress!,
            LoginInput.Password!,
            LoginInput.RememberMe,
            true
        );

        await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
        {
            Identity = IdentitySecurityLogIdentityConsts.Identity,
            Action = result.ToIdentitySecurityLogAction(),
            UserName = LoginInput.UserNameOrEmailAddress!
        });

        if (result.RequiresTwoFactor)
        {
            return await TwoFactorLoginResultAsync();
        }

        if (result.IsLockedOut)
        {
            SetLoginError(L["UserLockedOutMessage"]);
            return Page();
        }

        if (result.IsNotAllowed)
        {
            SetLoginError(L["LoginIsNotAllowed"]);
            return Page();
        }

        if (!result.Succeeded)
        {
            if (LoginInput.UserNameOrEmailAddress == IdentityDataSeedContributor.AdminUserNameDefaultValue &&
                WebHostEnvironment.IsDevelopment())
            {
                var adminUser = await UserManager.FindByNameAsync(IdentityDataSeedContributor.AdminUserNameDefaultValue);
                if (adminUser == null)
                {
                    ShowRequireMigrateSeedMessage = true;
                    return Page();
                }
            }

            SetLoginError(L["InvalidUserNameOrPassword"], true);
            return Page();
        }

        //TODO: Find a way of getting user's id from the logged in user and do not query it again like that!
        var user = resolvedUser;

        Debug.Assert(user != null, nameof(user) + " != null");

        // Clear the dynamic claims cache.
        await IdentityDynamicClaimsPrincipalContributorCache.ClearAsync(user.Id, user.TenantId);

        // Create session for the login
        await CreateSessionAsync(user);

        return await RedirectSafelyAsync(ReturnUrl ?? string.Empty, ReturnUrlHash);
    }

    /// <summary>
    /// OpenIddict 等场景下从 ReturnUrl 读取 __tenant；默认无。
    /// </summary>
    protected virtual Task<string?> GetOidcAuthorizationTenantParameterAsync()
    {
        return Task.FromResult<string?>(null);
    }

    /// <summary>
    /// 在密码校验前切换/清空当前租户，使 SignIn 与用户库 tenant_id 一致。
    /// </summary>
    protected virtual async Task InitializeTenantSelectionAsync()
    {
        var fromAuthorization = await GetOidcAuthorizationTenantParameterAsync();
        if (!string.IsNullOrWhiteSpace(fromAuthorization))
        {
            var trimmed = fromAuthorization.Trim();
            EnterpriseTenantCode = trimmed;
            IsEnterpriseTenantPresetFromLink = true;
            UseSpecifiedTenant = true;
        }
    }

    /// <summary>
    /// 异步尝试准备租户选择。
    /// </summary>
    /// <returns>异步操作结果。</returns>
    protected virtual async Task<bool> TryPrepareTenantSelectionAsync()
    {
        await InitializeTenantSelectionAsync();

        if (UseSpecifiedTenant)
        {
            if (string.IsNullOrWhiteSpace(EnterpriseTenantCode))
            {
                SetLoginError("请输入企业编码。");
                return false;
            }

            var tenant = await FindTenantByCodeOrDomainAsync(EnterpriseTenantCode.Trim());
            if (tenant == null)
            {
                SetLoginError("企业编码无效，请核对后重试。");
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Clear 租户 上下文 For Host 登录。
    /// </summary>
    protected virtual void ClearTenantContextForHostLogin()
    {
        CurrentTenant.Change(null);
        Response.Cookies.Delete(TenantResolverConsts.DefaultTenantKey);
    }

    /// <summary>
    /// 异步根据编码或域名查找租户。
    /// </summary>
    /// <param name="raw">raw。</param>
    /// <returns>异步操作结果。</returns>
    protected virtual async Task<Tenant?> FindTenantByCodeOrDomainAsync(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        var tenantRepository = LazyServiceProvider.LazyGetRequiredService<ITenantRepository>();
        return await tenantRepository.FindByCodeAsync(raw.Trim())
            ?? await tenantRepository.FindByDomainAsync(raw.Trim());
    }

    /// <summary>
    /// Apply 租户 上下文。
    /// </summary>
    /// <param name="tenantId">租户标识。</param>
    protected virtual void ApplyTenantContext(Guid? tenantId)
    {
        if (!tenantId.HasValue)
        {
            ClearTenantContextForHostLogin();
            return;
        }

        CurrentTenant.Change(tenantId);
        Response.Cookies.Append(
            TenantResolverConsts.DefaultTenantKey,
            tenantId.Value.ToString("D"),
            new CookieOptions
            {
                IsEssential = true,
                HttpOnly = false,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Lax,
                Path = "/"
            });
    }

    /// <summary>
    /// 异步重新加载登录页面状态。
    /// </summary>
    /// <returns>表示异步操作的任务。</returns>
    protected virtual async Task ReloadLoginPageStateAsync()
    {
        ExternalProviders = await GetExternalProviders();
        EnableLocalLogin = await SettingProvider.IsTrueAsync(AccountSettingNames.EnableLocalLogin);
        EnableRememberMe = await SettingProvider.IsTrueAsync(CenseqAccountSettingNames.EnableRememberMe);
        EnableCaptcha = await SettingProvider.IsTrueAsync(CenseqAccountSettingNames.EnableCaptcha);
        IsSelfRegistrationEnabled = await SettingProvider.IsTrueAsync(AccountSettingNames.IsSelfRegistrationEnabled);
    }

    /// <summary>
    /// 异步验证验证码。
    /// </summary>
    /// <returns>异步操作结果。</returns>
    protected virtual async Task<bool> ValidateCaptchaAsync()
    {
        if (!await IsCaptchaEnabledAsync())
        {
            return true;
        }

        if (LoginInput.CaptchaId.IsNullOrWhiteSpace() || LoginInput.CaptchaCode.IsNullOrWhiteSpace())
        {
            SetLoginError("请输入验证码。");
            return false;
        }

        var captcha = LazyServiceProvider.LazyGetRequiredService<ICaptcha>();
        if (captcha.Validate(LoginInput.CaptchaId, LoginInput.CaptchaCode))
        {
            return true;
        }

        SetLoginError("验证码错误或已过期，请重新输入。", true);
        return false;
    }

    /// <summary>
    /// 异步判断是否启用验证码。
    /// </summary>
    /// <returns>异步操作结果。</returns>
    protected virtual async Task<bool> IsCaptchaEnabledAsync()
    {
        return await SettingProvider.IsTrueAsync(CenseqAccountSettingNames.EnableCaptcha);
    }

    /// <summary>
    /// Override this method to add 2FA for your application.
    /// </summary>
    protected virtual Task<IActionResult> TwoFactorLoginResultAsync()
    {
        throw new NotImplementedException();
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
    public virtual async Task<IActionResult> OnPostExternalLogin(string provider)
    {
        var redirectUrl = Url.Page("./Login", pageHandler: "ExternalLoginCallback", values: new { ReturnUrl, ReturnUrlHash });
        var properties = SignInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        properties.Items["scheme"] = provider;

        return await Task.FromResult(Challenge(properties, provider));
    }

    /// <summary>
    /// 异步处理外部登录回调。
    /// </summary>
    /// <param name="returnUrl">返回地址。</param>
    /// <param name="returnUrlHash">返回地址哈希。</param>
    /// <param name="remoteError">远程 Error。</param>
    /// <returns>页面处理结果。</returns>
    public virtual async Task<IActionResult> OnGetExternalLoginCallbackAsync(string returnUrl = "", string returnUrlHash = "", string? remoteError = null)
    {
        //TODO: Did not implemented Identity Server 4 sample for this method (see ExternalLoginCallback in Quickstart of IDS4 sample)
        /* Also did not implement these:
         * - Logout(string logoutId)
         */

        if (remoteError != null)
        {
            Logger.LogWarning($"External login callback error: {remoteError}");
            return RedirectToPage("./Login");
        }

        await IdentityOptions.SetAsync();

        var loginInfo = await SignInManager.GetExternalLoginInfoAsync();
        if (loginInfo == null)
        {
            Logger.LogWarning("External login info is not available");
            return RedirectToPage("./Login");
        }

        var result = await SignInManager.ExternalLoginSignInAsync(
            loginInfo.LoginProvider,
            loginInfo.ProviderKey,
            isPersistent: false,
            bypassTwoFactor: true
        );

        if (!result.Succeeded)
        {
            await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
            {
                Identity = IdentitySecurityLogIdentityConsts.IdentityExternal,
                Action = "Login" + result
            });
        }

        if (result.IsLockedOut)
        {
            Logger.LogWarning($"External login callback error: user is locked out!");
            throw new UserFriendlyException("Cannot proceed because user is locked out!");
        }

        if (result.IsNotAllowed)
        {
            Logger.LogWarning($"External login callback error: user is not allowed!");
            throw new UserFriendlyException("Cannot proceed because user is not allowed!");
        }

        IdentityUser? user;
        if (result.Succeeded)
        {
            user = await UserManager.FindByLoginAsync(loginInfo.LoginProvider, loginInfo.ProviderKey);
            if (user != null)
            {
                // Clear the dynamic claims cache.
                await IdentityDynamicClaimsPrincipalContributorCache.ClearAsync(user.Id, user.TenantId);
            }

            return await RedirectSafelyAsync(returnUrl, returnUrlHash);
        }

        //TODO: Handle other cases for result!

        var email = loginInfo.Principal.FindFirstValue(AbpClaimTypes.Email) ?? loginInfo.Principal.FindFirstValue(ClaimTypes.Email);
        if (email.IsNullOrWhiteSpace())
        {
            return RedirectToPage("./Register", new {
                IsExternalLogin = true,
                ExternalLoginAuthSchema = loginInfo.LoginProvider,
                ReturnUrl = returnUrl
            });
        }

        user = await UserManager.FindByEmailAsync(email);
        if (user == null)
        {
            return RedirectToPage("./Register", new {
                IsExternalLogin = true,
                ExternalLoginAuthSchema = loginInfo.LoginProvider,
                ReturnUrl = returnUrl
            });
        }

        if (await UserManager.FindByLoginAsync(loginInfo.LoginProvider, loginInfo.ProviderKey) == null)
        {
            CheckIdentityErrors(await UserManager.AddLoginAsync(user, loginInfo));
        }

        await SignInManager.SignInAsync(user, false);

        await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
        {
            Identity = IdentitySecurityLogIdentityConsts.IdentityExternal,
            Action = result.ToIdentitySecurityLogAction(),
            UserName = user.Name ?? user.UserName ?? string.Empty
        });

        // Clear the dynamic claims cache.
        await IdentityDynamicClaimsPrincipalContributorCache.ClearAsync(user.Id, user.TenantId);

        return await RedirectSafelyAsync(returnUrl, returnUrlHash);
    }

    /// <summary>
    /// 异步解析登录用户。
    /// </summary>
    /// <returns>登录用户解析结果。</returns>
    protected virtual async Task<LoginUserResolutionResult> ResolveLoginUserAsync()
    {
        var loginName = LoginInput.UserNameOrEmailAddress?.Trim();
        if (loginName.IsNullOrWhiteSpace())
        {
            return LoginUserResolutionResult.Failed();
        }

        var specifiedTenant = await GetSpecifiedTenantAsync();
        if (UseSpecifiedTenant && specifiedTenant == null)
        {
            return LoginUserResolutionResult.Failed();
        }

        List<IdentityUser> candidates;
        var isEmailLogin = ValidationHelper.IsValidEmailAddress(loginName);
        var userRepository = LazyServiceProvider.LazyGetRequiredService<IIdentityUserRepository>();

        using (LazyServiceProvider.LazyGetRequiredService<IDataFilter>().Disable<Volo.Abp.MultiTenancy.IMultiTenant>())
        {
            if (isEmailLogin)
            {
                candidates = await userRepository.GetListAsync(
                    emailAddress: loginName,
                    includeDetails: false);
            }
            else
            {
                candidates = await userRepository.GetListAsync(
                    userName: loginName,
                    includeDetails: false);
            }
        }

        if (specifiedTenant != null)
        {
            candidates = candidates.Where(x => x.TenantId == specifiedTenant.Id).ToList();
        }

        if (!isEmailLogin && !UseSpecifiedTenant && candidates.Count > 1)
        {
            SetLoginError("该账号存在多个企业，请勾选“指定企业编码”并输入企业编码后再登录。");
            return LoginUserResolutionResult.Failed();
        }

        var matchedUsers = new List<IdentityUser>();
        foreach (var candidate in candidates)
        {
            if (await UserManager.CheckPasswordAsync(candidate, LoginInput.Password!))
            {
                matchedUsers.Add(candidate);
            }
        }

        if (matchedUsers.Count == 0)
        {
            SetLoginError(L["InvalidUserNameOrPassword"], true);
            return LoginUserResolutionResult.Failed();
        }

        if (isEmailLogin && matchedUsers.Count > 1)
        {
            return LoginUserResolutionResult.Redirect(await CreateTenantSelectionRedirectAsync(loginName, matchedUsers));
        }

        if (!isEmailLogin && matchedUsers.Count > 1)
        {
            SetLoginError("该账号匹配到多个企业，请指定企业编码后重试。");
            return LoginUserResolutionResult.Failed();
        }

        var resolvedUser = matchedUsers[0];
        ApplyTenantContext(resolvedUser.TenantId);
        LoginInput.UserNameOrEmailAddress = resolvedUser.UserName;
        return LoginUserResolutionResult.Success(resolvedUser);
    }

    /// <summary>
    /// 异步创建租户选择重定向。
    /// </summary>
    /// <param name="loginName">登录 Name。</param>
    /// <param name="matchedUsers">matched Users。</param>
    /// <returns>异步操作结果。</returns>
    protected virtual async Task<IActionResult> CreateTenantSelectionRedirectAsync(string loginName, List<IdentityUser> matchedUsers)
    {
        var cache = LazyServiceProvider.LazyGetRequiredService<IDistributedCache<LoginTenantSelectionCacheItem>>();
        var token = Guid.NewGuid().ToString("N");
        var options = await BuildTenantSelectionOptionsAsync(matchedUsers);
        var cacheItem = new LoginTenantSelectionCacheItem
        {
            LoginName = loginName,
            RememberMe = LoginInput.RememberMe,
            ReturnUrl = ReturnUrl,
            ReturnUrlHash = ReturnUrlHash,
            Options = options
        };

        await cache.SetAsync(
            token,
            cacheItem,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            },
            considerUow: true);

        return RedirectToPage("./SelectEnterprise", new
        {
            token,
            returnUrl = ReturnUrl,
            returnUrlHash = ReturnUrlHash
        });
    }

    /// <summary>
    /// 异步构建租户选择配置。
    /// </summary>
    /// <param name="matchedUsers">matched Users。</param>
    /// <returns>异步操作结果。</returns>
    protected virtual async Task<List<LoginTenantSelectionOption>> BuildTenantSelectionOptionsAsync(List<IdentityUser> matchedUsers)
    {
        var options = new List<LoginTenantSelectionOption>();
        var tenantRepository = LazyServiceProvider.LazyGetRequiredService<ITenantRepository>();
        var tenantIds = matchedUsers
            .Where(x => x.TenantId.HasValue)
            .Select(x => x.TenantId!.Value)
            .Distinct()
            .ToList();
        var tenantLookup = new Dictionary<Guid, Tenant>();

        using (CurrentTenant.Change(null))
        {
            foreach (var tenantId in tenantIds)
            {
                var tenant = await tenantRepository.FindAsync(tenantId);
                if (tenant != null)
                {
                    tenantLookup[tenantId] = tenant;
                }
            }
        }

        foreach (var matchedUser in matchedUsers)
        {
            var isHost = !matchedUser.TenantId.HasValue;
            tenantLookup.TryGetValue(matchedUser.TenantId ?? Guid.Empty, out var tenant);
            options.Add(new LoginTenantSelectionOption
            {
                UserId = matchedUser.Id,
                ActualTenantId = matchedUser.TenantId,
                TenantId = matchedUser.TenantId ?? Guid.Empty,
                TenantCode = isHost ? "Default" : tenant?.Code ?? string.Empty,
                TenantName = isHost ? "平台系统" : tenant?.Name ?? "未命名企业",
                UserName = matchedUser.UserName ?? string.Empty,
                DisplayName = matchedUser.Name ?? matchedUser.UserName ?? matchedUser.Email ?? string.Empty,
                Email = matchedUser.Email ?? string.Empty
            });
        }

        return options;
    }

    /// <summary>
    /// 异步获取指定租户。
    /// </summary>
    /// <returns>租户信息。</returns>
    protected virtual async Task<Tenant?> GetSpecifiedTenantAsync()
    {
        if (!UseSpecifiedTenant)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(EnterpriseTenantCode))
        {
            return null;
        }

        return await FindTenantByCodeOrDomainAsync(EnterpriseTenantCode.Trim());
    }

    /// <summary>
    /// 设置登录错误信息。
    /// </summary>
    /// <param name="message">message。</param>
    /// <param name="isDanger">is Danger。</param>
    protected virtual void SetLoginError(string message, bool isDanger = false)
    {
        LoginErrorMessage = message;
        if (isDanger)
        {
            Alerts.Danger(message);
            return;
        }

        Alerts.Warning(message);
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

    /// <summary>
    /// 异步创建会话。
    /// </summary>
    /// <param name="user">用户。</param>
    /// <returns>表示异步操作的任务。</returns>
    protected virtual async Task CreateSessionAsync(IdentityUser user)
    {
        try
        {
            var device = GetDeviceType();
            var deviceInfo = GetDeviceInfo();
            var clientId = "Admin_Web"; // Web登录的客户端ID

            // 获取IP地址
            var ipAddresses = GetClientIpAddresses();

            await IdentitySessionManager.CreateAsync(
                user.Id,
                device,
                deviceInfo,
                clientId,
                ipAddresses
            );

            Logger.LogDebug("Created session for user {UserId} during login", user.Id);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to create session for user {UserId} during login", user.Id);
            // 不影响登录流程
        }
    }

    /// <summary>
    /// 获取设备类型。
    /// </summary>
    /// <returns>返回结果。</returns>
    protected virtual string GetDeviceType()
    {
        var userAgent = Request.Headers.UserAgent.ToString();
        if (string.IsNullOrEmpty(userAgent))
        {
            return IdentitySessionDevices.Web;
        }

        userAgent = userAgent.ToLowerInvariant();
        if (userAgent.Contains("mobile") || userAgent.Contains("android") || userAgent.Contains("iphone"))
        {
            return IdentitySessionDevices.Mobile;
        }

        return IdentitySessionDevices.Web;
    }

    /// <summary>
    /// Get Device 信息。
    /// </summary>
    /// <returns>返回结果。</returns>
    protected virtual string GetDeviceInfo()
    {
        try
        {
            var options = LazyServiceProvider.LazyGetRequiredService<IOptions<IdentitySessionOptions>>().Value;
            if (!options.SaveDeviceInfo)
            {
                return string.Empty;
            }

            var userAgent = Request.Headers.UserAgent.ToString();
            return userAgent?.Length > IdentitySessionConsts.MaxDeviceInfoLength
                ? userAgent[..IdentitySessionConsts.MaxDeviceInfoLength]
                : userAgent ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Get Client Ip Addresses。
    /// </summary>
    /// <returns>返回结果。</returns>
    protected virtual string GetClientIpAddresses()
    {
        try
        {
            var ips = new List<string>();

            // 尝试获取 X-Forwarded-For 头（经过代理时）
            var forwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(forwardedFor))
            {
                ips.AddRange(forwardedFor.Split(',').Select(ip => ip.Trim()).Where(ip => !string.IsNullOrWhiteSpace(ip)));
            }

            // 添加远程 IP
            var remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            if (!string.IsNullOrWhiteSpace(remoteIp) && !ips.Contains(remoteIp))
            {
                ips.Add(remoteIp);
            }

            return string.Join(",", ips);
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// 登录输入模型。
    /// </summary>
    public class LoginInputModel
    {
        /// <summary>
        /// 用户名或邮箱地址。
        /// </summary>
        [Required]
        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxEmailLength))]
        public string? UserNameOrEmailAddress { get; set; }

        /// <summary>
        /// 密码。
        /// </summary>
        [Required]
        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPasswordLength))]
        [DataType(DataType.Password)]
        [DisableAuditing]
        public string? Password { get; set; }

        /// <summary>
        /// 验证码标识。
        /// </summary>
        [DisableAuditing]
        public string? CaptchaId { get; set; }

        /// <summary>
        /// 验证码。
        /// </summary>
        [Display(Name = "验证码")]
        [DisableAuditing]
        public string? CaptchaCode { get; set; }

        /// <summary>
        /// 是否记住登录。
        /// </summary>
        public bool RememberMe { get; set; }
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

    /// <summary>
    /// 登录用户解析结果。
    /// </summary>
    public class LoginUserResolutionResult
    {
        /// <summary>
        /// 用户。
        /// </summary>
        public IdentityUser? User { get; init; }
        /// <summary>
        /// 重定向结果。
        /// </summary>
        public IActionResult? RedirectResult { get; init; }

        /// <summary>
        /// 返回登录成功结果。
        /// </summary>
        /// <param name="user">用户。</param>
        /// <returns>登录结果。</returns>
        public static LoginUserResolutionResult Success(IdentityUser user)
        {
            return new LoginUserResolutionResult { User = user };
        }

        /// <summary>
        /// Failed。
        /// </summary>
        /// <returns>返回结果。</returns>
        public static LoginUserResolutionResult Failed()
        {
            return new LoginUserResolutionResult();
        }

        /// <summary>
        /// 执行重定向。
        /// </summary>
        /// <param name="redirectResult">redirect 结果。</param>
        /// <returns>返回结果。</returns>
        public static LoginUserResolutionResult Redirect(IActionResult redirectResult)
        {
            return new LoginUserResolutionResult { RedirectResult = redirectResult };
        }
    }

    /// <summary>
    /// 登录租户选择缓存项。
    /// </summary>
    [Serializable]
    public class LoginTenantSelectionCacheItem
    {
        /// <summary>
        /// 登录名称。
        /// </summary>
        public string LoginName { get; set; } = string.Empty;
        /// <summary>
        /// 是否记住登录。
        /// </summary>
        public bool RememberMe { get; set; }
        /// <summary>
        /// 返回地址。
        /// </summary>
        public string? ReturnUrl { get; set; }
        /// <summary>
        /// 返回地址哈希。
        /// </summary>
        public string? ReturnUrlHash { get; set; }
        /// <summary>
        /// 配置项。
        /// </summary>
        public List<LoginTenantSelectionOption> Options { get; set; } = new();
    }

    /// <summary>
    /// 登录租户选择项。
    /// </summary>
    [Serializable]
    public class LoginTenantSelectionOption
    {
        /// <summary>
        /// 用户标识。
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 实际租户标识。
        /// </summary>
        public Guid? ActualTenantId { get; set; }
        /// <summary>
        /// 租户标识。
        /// </summary>
        public Guid TenantId { get; set; }
        /// <summary>
        /// 租户编码。
        /// </summary>
        public string TenantCode { get; set; } = string.Empty;
        /// <summary>
        /// 租户名称。
        /// </summary>
        public string TenantName { get; set; } = string.Empty;
        /// <summary>
        /// 用户名。
        /// </summary>
        public string UserName { get; set; } = string.Empty;
        /// <summary>
        /// 显示名称。
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;
        /// <summary>
        /// 邮箱。
        /// </summary>
        public string Email { get; set; } = string.Empty;
    }
}
