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

[ExposeServices(typeof(LoginModel))]
public class OpenIddictSupportedLoginModel : LoginModel
{
    protected CenseqOpenIddictRequestHelper OpenIddictRequestHelper { get; }

    public OpenIddictSupportedLoginModel(
        IAuthenticationSchemeProvider schemeProvider,
        IOptions<CenseqAccountOptions> accountOptions,
        IdentityDynamicClaimsPrincipalContributorCache identityDynamicClaimsPrincipalContributorCache,
        CenseqOpenIddictRequestHelper openIddictRequestHelper,
        IWebHostEnvironment webHostEnvironment)
        : base(schemeProvider, accountOptions, identityDynamicClaimsPrincipalContributorCache, webHostEnvironment)
    {
        OpenIddictRequestHelper = openIddictRequestHelper;
    }

    protected override async Task<string?> GetOidcAuthorizationTenantParameterAsync()
    {
        var request = await OpenIddictRequestHelper.GetFromReturnUrlAsync(ReturnUrl ?? string.Empty);
        var tenant = request?.GetParameter(TenantResolverConsts.DefaultTenantKey)?.ToString();
        return string.IsNullOrWhiteSpace(tenant) ? null : tenant;
    }

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
            AutoSelectEnterpriseTab = true;
            LoginScope = "enterprise";
        }

        return page;
    }

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

    public async override Task<IActionResult> OnPostExternalLogin(string provider)
    {
        if (AccountOptions.WindowsAuthenticationSchemeName == provider)
        {
            return await ProcessWindowsLoginAsync();
        }

        return await base.OnPostExternalLogin(provider);
    }

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
