using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Server;
using OpenIddict.Server.AspNetCore;
using Volo.Abp.DependencyInjection;
using Censeq.Identity;
using Volo.Abp.MultiTenancy;
using Censeq.OpenIddict;

namespace Censeq.Account.Web.Pages.Account;

/// <summary>
/// 支持 OpenIddict 的登录页面模型。
/// </summary>
[ExposeServices(typeof(LoginModel))]
public class OpenIddictSupportedLoginModel : LoginModel
{
    /// <summary>
    /// OpenIddict 请求辅助器。
    /// </summary>
    protected CenseqOpenIddictRequestHelper OpenIddictRequestHelper { get; }

    /// <summary>
    /// 初始化 OpenIddictSupportedLoginModel 实例。
    /// </summary>
    /// <param name="schemeProvider">认证方案提供者。</param>
    /// <param name="accountOptions">账户配置项。</param>
    /// <param name="identityDynamicClaimsPrincipalContributorCache">Identity 动态声明主体贡献器缓存。</param>
    /// <param name="openIddictRequestHelper">OpenIddict 请求辅助器。</param>
    /// <param name="webHostEnvironment">Web 主机环境。</param>
    /// <param name="identitySessionManager">Identity 会话管理器。</param>
    public OpenIddictSupportedLoginModel(
        IAuthenticationSchemeProvider schemeProvider,
        IOptions<CenseqAccountOptions> accountOptions,
        IdentityDynamicClaimsPrincipalContributorCache identityDynamicClaimsPrincipalContributorCache,
        CenseqOpenIddictRequestHelper openIddictRequestHelper,
        IWebHostEnvironment webHostEnvironment,
        IdentitySessionManager identitySessionManager)
        : base(schemeProvider, accountOptions, identityDynamicClaimsPrincipalContributorCache, webHostEnvironment, identitySessionManager)
    {
        OpenIddictRequestHelper = openIddictRequestHelper;
    }

    /// <summary>
    /// 异步获取 OIDC 授权租户参数。
    /// </summary>
    /// <returns>异步操作结果。</returns>
    protected override async Task<string?> GetOidcAuthorizationTenantParameterAsync()
    {
        var request = await OpenIddictRequestHelper.GetFromReturnUrlAsync(ReturnUrl ?? string.Empty);
        var tenant = request?.GetParameter(TenantResolverConsts.DefaultTenantKey)?.ToString();
        return string.IsNullOrWhiteSpace(tenant) ? null : tenant;
    }

    /// <summary>
    /// 异步处理页面 GET 请求。
    /// </summary>
    /// <returns>页面处理结果。</returns>
    public override async Task<IActionResult> OnGetAsync()
    {
        var page = await base.OnGetAsync();
        if (page is not PageResult)
        {
            return page;
        }

        var request = await OpenIddictRequestHelper.GetFromReturnUrlAsync(ReturnUrl ?? string.Empty);
        if (request?.ClientId != null && !string.IsNullOrEmpty(request.LoginHint))
        {
            LoginInput.UserNameOrEmailAddress = request.LoginHint;
        }

        var tenant = request?.GetParameter(TenantResolverConsts.DefaultTenantKey)?.ToString();
        if (!string.IsNullOrWhiteSpace(tenant))
        {
            EnterpriseTenantCode = tenant.Trim();
            IsEnterpriseTenantPresetFromLink = true;
            UseSpecifiedTenant = true;
        }

        return page;
    }

    /// <summary>
    /// 异步处理页面 POST 请求。
    /// </summary>
    /// <param name="action">action。</param>
    /// <returns>页面处理结果。</returns>
    public async override Task<IActionResult> OnPostAsync(string action)
    {
        if (action == "Cancel")
        {
            var request = await OpenIddictRequestHelper.GetFromReturnUrlAsync(ReturnUrl ?? string.Empty);

            var transaction = HttpContext.GetOpenIddictServerTransaction();
            if (request?.ClientId != null && transaction != null)
            {
                transaction.EndpointType = OpenIddictServerEndpointType.Authorization;
                transaction.Request = request;

                var notification = new OpenIddictServerEvents.ValidateAuthorizationRequestContext(transaction);
                transaction.SetProperty(typeof(OpenIddictServerEvents.ValidateAuthorizationRequestContext).FullName!, notification);

                return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            return Redirect("~/");
        }

        return await base.OnPostAsync(action);
    }

    /// <summary>
    /// 处理外部登录提交。
    /// </summary>
    /// <param name="provider">提供者。</param>
    /// <returns>页面处理结果。</returns>
    public async override Task<IActionResult> OnPostExternalLogin(string provider)
    {
        if (AccountOptions.WindowsAuthenticationSchemeName == provider)
        {
            return await ProcessWindowsLoginAsync();
        }

        return await base.OnPostExternalLogin(provider);
    }

    /// <summary>
    /// 异步处理 Windows 登录。
    /// </summary>
    /// <returns>异步操作结果。</returns>
    protected virtual async Task<IActionResult> ProcessWindowsLoginAsync()
    {
        var result = await HttpContext.AuthenticateAsync(AccountOptions.WindowsAuthenticationSchemeName);
        if (result.Succeeded)
        {
            var props = new AuthenticationProperties()
            {
                RedirectUri = Url.Page("./Login", pageHandler: "ExternalLoginCallback", values: new { ReturnUrl, ReturnUrlHash }),
                Items =
                {
                    {
                        "LoginProvider", AccountOptions.WindowsAuthenticationSchemeName
                    }
                }
            };

            var id = new ClaimsIdentity(AccountOptions.WindowsAuthenticationSchemeName);
            id.AddClaim(new Claim(ClaimTypes.NameIdentifier, result.Principal.FindFirstValue(ClaimTypes.PrimarySid) ?? string.Empty));
            id.AddClaim(new Claim(ClaimTypes.Name, result.Principal.FindFirstValue(ClaimTypes.Name) ?? string.Empty));

            await HttpContext.SignInAsync(IdentityConstants.ExternalScheme, new ClaimsPrincipal(id), props);

            return Redirect(props.RedirectUri!);
        }

        return Challenge(AccountOptions.WindowsAuthenticationSchemeName);
    }
}
