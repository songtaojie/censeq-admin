using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Censeq.Identity;
using Censeq.Identity.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using Volo.Abp.Security.Claims;
using IdentityUser = Censeq.Identity.Entities.IdentityUser;

namespace Censeq.OpenIddict.Controllers;

public partial class TokenController
{
    protected virtual async Task<IActionResult> HandleAuthorizationCodeAsync(OpenIddictRequest request)
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

            // Create session and add SessionId claim
            var session = await CreateSessionForAuthorizationCodeAsync(user, request);
            if (session != null)
            {
                var sessionIdClaim = new Claim(AbpClaimTypes.SessionId, session.SessionId)
                    .SetDestinations(OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken);
                principal.Identities.FirstOrDefault()!.AddClaim(sessionIdClaim);
            }

            // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
    }

    protected virtual async Task<IdentitySession?> CreateSessionForAuthorizationCodeAsync(IdentityUser user, OpenIddictRequest request)
    {
        try
        {
            var device = GetDeviceType();
            var deviceInfo = GetDeviceInfo();
            var clientId = request.ClientId ?? string.Empty;

            // 检查并限制最大并发会话数
            var options = LazyServiceProvider.LazyGetRequiredService<IOptions<IdentitySessionOptions>>().Value;
            if (options.MaxConcurrentSessions.HasValue && options.MaxConcurrentSessions.Value > 0)
            {
                var currentCount = await IdentitySessionManager.GetCountAsync(user.Id);
                if (currentCount >= options.MaxConcurrentSessions.Value && options.AutoRemoveOldestSession)
                {
                    // 删除最旧的会话
                    await IdentitySessionManager.DeleteAllAsync(user.Id);
                }
            }

            var ipAddresses = GetClientIpAddresses();

            return await IdentitySessionManager.CreateAsync(
                user.Id,
                device,
                deviceInfo,
                clientId,
                ipAddresses
            );
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to create session for user {UserId} in authorization code flow", user.Id);
            return null;
        }
    }
}
