using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using Volo.Abp.Security.Claims;

namespace Censeq.OpenIddict.Controllers;

/// <summary>
/// 令牌控制器，提供对应的 HTTP API。
/// </summary>
public partial class TokenController
{
    /// <summary>
    /// 处理当前请求。
    /// </summary>
    /// <param name="request">OpenIddict 请求。</param>
    /// <returns>异步操作结果。</returns>
    protected virtual async Task<IActionResult> HandleRefreshTokenAsync(OpenIddictRequest request)
    {
        // Retrieve the claims principal stored in the authorization code/device code/refresh token.
        var principal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;
        principal = await AbpClaimsPrincipalFactory.CreateDynamicAsync(principal);
        using (CurrentTenant.Change(principal.FindTenantId()))
        {
            // Retrieve the user profile corresponding to the authorization code/refresh token.
            // Note: if you want to automatically invalidate the authorization code/refresh token
            // when the user password/roles change, use the following line instead:
            // var user = _signInManager.ValidateSecurityStampAsync(info.Principal);
            var user = await UserManager.GetUserAsync(principal);
            if (user == null)
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The token is no longer valid."
                    }));
            }

            // Ensure the user is still allowed to sign in.
            if (!await PreSignInCheckAsync(user))
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user is no longer allowed to sign in."
                    }));
            }

            await OpenIddictClaimsPrincipalManager.HandleAsync(request, principal);

            // 更新现有session的最后访问时间
            await UpdateSessionLastAccessedTimeAsync(principal);

            // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
    }

    /// <summary>
    /// 异步更新会话最后访问时间。
    /// </summary>
    /// <param name="principal">主体。</param>
    /// <returns>表示异步操作的任务。</returns>
    protected virtual async Task UpdateSessionLastAccessedTimeAsync(ClaimsPrincipal principal)
    {
        try
        {
            var sessionId = principal.FindFirstValue(AbpClaimTypes.SessionId);
            if (!string.IsNullOrEmpty(sessionId))
            {
                await IdentitySessionManager.UpdateLastAccessedTimeAsync(sessionId);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to update session last accessed time during token refresh");
        }
    }
}
