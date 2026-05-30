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

public class LoginModel : AccountPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string? ReturnUrlHash { get; set; }

    [BindProperty]
    public LoginInputModel LoginInput { get; set; } = default!;

    [BindProperty]
    [Display(Name = "指定企业编码")]
    public bool UseSpecifiedTenant { get; set; }

    [BindProperty]
    public string? EnterpriseTenantCode { get; set; }

    /// <summary>授权链接带 __tenant 时为 true，租户编码只读并固定使用链接值。</summary>
    public bool IsEnterpriseTenantPresetFromLink { get; set; }

    public bool EnableLocalLogin { get; set; }

    public bool EnableRememberMe { get; set; }

    public bool EnableCaptcha { get; set; }

    public bool IsSelfRegistrationEnabled { get; set; }

    //TODO: Why there is an ExternalProviders if only the VisibleExternalProviders is used.
    public IEnumerable<ExternalProviderModel>? ExternalProviders { get; set; }
    public IEnumerable<ExternalProviderModel>? VisibleExternalProviders => ExternalProviders?.Where(x => !string.IsNullOrWhiteSpace(x.DisplayName));

    public bool IsExternalLoginOnly => EnableLocalLogin == false && ExternalProviders?.Count() == 1;
    public string? ExternalLoginScheme => IsExternalLoginOnly ? ExternalProviders?.SingleOrDefault()?.AuthenticationScheme : null;

    //Optional IdentityServer services
    //public IIdentityServerInteractionService Interaction { get; set; }
    //public IClientStore ClientStore { get; set; }
    //public IEventService IdentityServerEvents { get; set; }

    protected IAuthenticationSchemeProvider SchemeProvider { get; }
    protected CenseqAccountOptions AccountOptions { get; }
    //protected IOptions<IdentityOptions> IdentityOptions { get; }
    protected IdentityDynamicClaimsPrincipalContributorCache IdentityDynamicClaimsPrincipalContributorCache { get; }
    protected IWebHostEnvironment WebHostEnvironment { get; }
    protected IdentitySessionManager IdentitySessionManager { get; }
    public bool ShowCancelButton { get; set; }
    public bool ShowRequireMigrateSeedMessage { get; set; }
    public string? LoginErrorMessage { get; set; }

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

    protected virtual void ClearTenantContextForHostLogin()
    {
        CurrentTenant.Change(null);
        Response.Cookies.Delete(TenantResolverConsts.DefaultTenantKey);
    }

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

    protected virtual async Task ReloadLoginPageStateAsync()
    {
        ExternalProviders = await GetExternalProviders();
        EnableLocalLogin = await SettingProvider.IsTrueAsync(AccountSettingNames.EnableLocalLogin);
        EnableRememberMe = await SettingProvider.IsTrueAsync(CenseqAccountSettingNames.EnableRememberMe);
        EnableCaptcha = await SettingProvider.IsTrueAsync(CenseqAccountSettingNames.EnableCaptcha);
        IsSelfRegistrationEnabled = await SettingProvider.IsTrueAsync(AccountSettingNames.IsSelfRegistrationEnabled);
    }

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

    public virtual async Task<IActionResult> OnPostExternalLogin(string provider)
    {
        var redirectUrl = Url.Page("./Login", pageHandler: "ExternalLoginCallback", values: new { ReturnUrl, ReturnUrlHash });
        var properties = SignInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        properties.Items["scheme"] = provider;

        return await Task.FromResult(Challenge(properties, provider));
    }

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

    protected virtual async Task CheckLocalLoginAsync()
    {
        if (!await SettingProvider.IsTrueAsync(AccountSettingNames.EnableLocalLogin))
        {
            throw new UserFriendlyException(L["LocalLoginDisabledMessage"]);
        }
    }

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

    public class LoginInputModel
    {
        [Required]
        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxEmailLength))]
        public string? UserNameOrEmailAddress { get; set; }

        [Required]
        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPasswordLength))]
        [DataType(DataType.Password)]
        [DisableAuditing]
        public string? Password { get; set; }

        [DisableAuditing]
        public string? CaptchaId { get; set; }

        [Display(Name = "验证码")]
        [DisableAuditing]
        public string? CaptchaCode { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ExternalProviderModel
    {
        public required string DisplayName { get; set; }
        public string? AuthenticationScheme { get; set; }
    }

    public class LoginUserResolutionResult
    {
        public IdentityUser? User { get; init; }
        public IActionResult? RedirectResult { get; init; }

        public static LoginUserResolutionResult Success(IdentityUser user)
        {
            return new LoginUserResolutionResult { User = user };
        }

        public static LoginUserResolutionResult Failed()
        {
            return new LoginUserResolutionResult();
        }

        public static LoginUserResolutionResult Redirect(IActionResult redirectResult)
        {
            return new LoginUserResolutionResult { RedirectResult = redirectResult };
        }
    }

    [Serializable]
    public class LoginTenantSelectionCacheItem
    {
        public string LoginName { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }
        public string? ReturnUrlHash { get; set; }
        public List<LoginTenantSelectionOption> Options { get; set; } = new();
    }

    [Serializable]
    public class LoginTenantSelectionOption
    {
        public Guid UserId { get; set; }
        public Guid? ActualTenantId { get; set; }
        public Guid TenantId { get; set; }
        public string TenantCode { get; set; } = string.Empty;
        public string TenantName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
