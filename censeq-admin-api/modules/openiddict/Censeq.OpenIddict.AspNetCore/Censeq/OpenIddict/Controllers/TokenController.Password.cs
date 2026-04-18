using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Censeq.Identity;
using Censeq.Identity.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;
using Volo.Abp.Settings;
using Volo.Abp.Uow;
using Volo.Abp.Validation;
using IdentityUser = Censeq.Identity.Entities.IdentityUser;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Censeq.OpenIddict.Controllers;

public partial class TokenController
{
    protected IServiceScopeFactory ServiceScopeFactory => LazyServiceProvider.LazyGetRequiredService<IServiceScopeFactory>();
    protected ITenantConfigurationProvider TenantConfigurationProvider=> LazyServiceProvider.LazyGetRequiredService<ITenantConfigurationProvider>();
    protected IOptions<CenseqIdentityOptions> CenseqIdentityOptions => LazyServiceProvider.LazyGetRequiredService<IOptions<CenseqIdentityOptions>>();
    protected IOptions<IdentityOptions> IdentityOptions => LazyServiceProvider.LazyGetRequiredService<IOptions<IdentityOptions>>();
    protected IdentitySecurityLogManager IdentitySecurityLogManager => LazyServiceProvider.LazyGetRequiredService<IdentitySecurityLogManager>();
    protected IdentitySessionManager IdentitySessionManager => LazyServiceProvider.LazyGetRequiredService<IdentitySessionManager>();

    protected ISettingProvider SettingProvider => LazyServiceProvider.LazyGetRequiredService<ISettingProvider>();
    protected IdentityDynamicClaimsPrincipalContributorCache IdentityDynamicClaimsPrincipalContributorCache => LazyServiceProvider.LazyGetRequiredService<IdentityDynamicClaimsPrincipalContributorCache>();

    [UnitOfWork]
    protected virtual async Task<IActionResult> HandlePasswordAsync(OpenIddictRequest request)
    {
        using (var scope = ServiceScopeFactory.CreateScope())
        {
            var tenant = await TenantConfigurationProvider.GetAsync(saveResolveResult: false);

            using (CurrentTenant.Change(tenant?.Id))
            {
                await ReplaceEmailToUsernameOfInputIfNeeds(request);

                IdentityUser? user = null;

                if (CenseqIdentityOptions.Value.ExternalLoginProviders.Any())
                {
                    foreach (var externalLoginProviderInfo in CenseqIdentityOptions.Value.ExternalLoginProviders.Values)
                    {
                        var externalLoginProvider = (IExternalLoginProvider)scope.ServiceProvider
                            .GetRequiredService(externalLoginProviderInfo.Type);

                        if (await externalLoginProvider.TryAuthenticateAsync(request.Username ?? string.Empty, request.Password ?? string.Empty))
                        {
                            user = await UserManager.FindByNameAsync(request.Username ?? string.Empty);
                            if (user == null)
                            {
                                user = await externalLoginProvider.CreateUserAsync(request.Username ?? string.Empty, externalLoginProviderInfo.Name);
                            }
                            else
                            {
                                await externalLoginProvider.UpdateUserAsync(user, externalLoginProviderInfo.Name);
                            }

                            return await SetSuccessResultAsync(request, user);
                        }
                    }
                }

                await IdentityOptions.SetAsync();

                user = await UserManager.FindByNameAsync(request.Username ?? string.Empty);
                if (user == null)
                {
                    Logger.LogInformation("No user found matching username: {username}", request.Username);

                    var properties = new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Invalid username or password!"
                    });

                    await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
                    {
                        Identity = OpenIddictSecurityLogIdentityConsts.OpenIddict,
                        Action = OpenIddictSecurityLogActionConsts.LoginInvalidUserName,
                        UserName = request.Username ?? string.Empty,
                        ClientId = request.ClientId ?? string.Empty
                    });

                    return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                }

                var result = await SignInManager.CheckPasswordSignInAsync(user, request.Password ?? string.Empty, true);
                if (!result.Succeeded)
                {
                    await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
                    {
                        Identity = OpenIddictSecurityLogIdentityConsts.OpenIddict,
                        Action = result.ToIdentitySecurityLogAction(),
                        UserName = request.Username ?? string.Empty,
                        ClientId = request.ClientId ?? string.Empty
                    });

                    string errorDescription;
                    if (result.IsLockedOut)
                    {
                        Logger.LogInformation("Authentication failed for username: {username}, reason: locked out", request.Username);
                        errorDescription = "The user account has been locked out due to invalid login attempts. Please wait a while and try again.";
                    }
                    else if (result.IsNotAllowed)
                    {
                        Logger.LogInformation("Authentication failed for username: {username}, reason: not allowed", request.Username);

                        if (user.ShouldChangePasswordOnNextLogin)
                        {
                            return await HandleShouldChangePasswordOnNextLoginAsync(request, user, request.Password ?? string.Empty);
                        }

                        if (await UserManager.ShouldPeriodicallyChangePasswordAsync(user))
                        {
                            return await HandlePeriodicallyChangePasswordAsync(request, user, request.Password ?? string.Empty);
                        }

                        if (user.IsActive)
                        {
                            return await HandleConfirmUserAsync(request, user);
                        }

                        errorDescription = "You are not allowed to login! Your account is inactive.";
                    }
                    else
                    {
                        Logger.LogInformation("Authentication failed for username: {username}, reason: invalid credentials", request.Username);
                        errorDescription = "Invalid username or password!";
                    }

                    var properties = new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = errorDescription
                    });

                    return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                }

                if (await IsTfaEnabledAsync(user))
                {
                    return await HandleTwoFactorLoginAsync(request, user);
                }

                return await SetSuccessResultAsync(request, user);
            }
        }
    }

    protected virtual async Task ReplaceEmailToUsernameOfInputIfNeeds(OpenIddictRequest request)
    {
        if (!ValidationHelper.IsValidEmailAddress(request.Username ?? string.Empty))
        {
            return;
        }

        var userByUsername = await UserManager.FindByNameAsync(request.Username ?? string.Empty);
        if (userByUsername != null)
        {
            return;
        }

        var userByEmail = await UserManager.FindByEmailAsync(request.Username ?? string.Empty);
        if (userByEmail == null)
        {
            return;
        }

        request.Username = userByEmail.UserName ?? string.Empty;
    }

    protected virtual async Task<IActionResult> HandleTwoFactorLoginAsync(OpenIddictRequest request, IdentityUser user)
    {
        var recoveryCode = request.GetParameter("RecoveryCode")?.ToString() ?? string.Empty;
        if (!recoveryCode.IsNullOrWhiteSpace())
        {
            var result = await UserManager.RedeemTwoFactorRecoveryCodeAsync(user, recoveryCode);
            if (result.Succeeded)
            {
                return await SetSuccessResultAsync(request, user);
            }

            var properties = new AuthenticationProperties(new Dictionary<string, string?>
            {
                [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Invalid recovery code!"
            });

            return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        var twoFactorProvider = request.GetParameter("TwoFactorProvider")?.ToString() ?? string.Empty;
        var twoFactorCode = request.GetParameter("TwoFactorCode")?.ToString() ?? string.Empty;
        if (!twoFactorProvider.IsNullOrWhiteSpace() && !twoFactorCode.IsNullOrWhiteSpace())
        {
            var providers = await UserManager.GetValidTwoFactorProvidersAsync(user);
            if (providers.Contains(twoFactorProvider) && await UserManager.VerifyTwoFactorTokenAsync(user, twoFactorProvider, twoFactorCode))
            {
                return await SetSuccessResultAsync(request, user);
            }

            await UserManager.AccessFailedAsync(user);

            Logger.LogInformation("Authentication failed for username: {username}, reason: InvalidAuthenticatorCode", request.Username);

            var properties = new AuthenticationProperties(new Dictionary<string, string?>
            {
                [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Invalid authenticator code!"
            });

            return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
        else
        {
            Logger.LogInformation("Authentication failed for username: {username}, reason: RequiresTwoFactor", request.Username);
            var twoFactorToken = await UserManager.GenerateUserTokenAsync(user, TokenOptions.DefaultProvider, nameof(SignInResult.RequiresTwoFactor));

            await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
            {
                Identity = OpenIddictSecurityLogIdentityConsts.OpenIddict,
                Action = OpenIddictSecurityLogActionConsts.LoginRequiresTwoFactor,
                UserName = request.Username ?? string.Empty,
                ClientId = request.ClientId ?? string.Empty
            });

            var properties = new AuthenticationProperties(
                items: new Dictionary<string, string?>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = CenseqErrorDescriptionConsts.RequiresTwoFactor
                },
                parameters: new Dictionary<string, object?>
                {
                    ["userId"] = user.Id.ToString("N"),
                    ["twoFactorToken"] = twoFactorToken
                });

            return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
    }

    protected virtual async Task<IActionResult> HandleShouldChangePasswordOnNextLoginAsync(OpenIddictRequest request, IdentityUser user, string currentPassword)
    {
        return await HandleChangePasswordAsync(request, user, currentPassword, ChangePasswordType.ShouldChangePasswordOnNextLogin);
    }

    protected virtual async Task<IActionResult> HandlePeriodicallyChangePasswordAsync(OpenIddictRequest request, IdentityUser user, string currentPassword)
    {
        return await HandleChangePasswordAsync(request, user, currentPassword, ChangePasswordType.PeriodicallyChangePassword);
    }

    protected virtual async Task<IActionResult> HandleChangePasswordAsync(OpenIddictRequest request, IdentityUser user, string currentPassword, ChangePasswordType changePasswordType)
    {
        var changePasswordToken = request.GetParameter("ChangePasswordToken")?.ToString();
        var newPassword = request.GetParameter("NewPassword")?.ToString();
        if (!changePasswordToken.IsNullOrWhiteSpace() && !currentPassword.IsNullOrWhiteSpace() && !newPassword.IsNullOrWhiteSpace())
        {
            if (await UserManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, changePasswordType.ToString(), changePasswordToken))
            {
                var changePasswordResult = await UserManager.ChangePasswordAsync(user, currentPassword, newPassword);
                if (changePasswordResult.Succeeded)
                {
                    await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
                    {
                        Identity = OpenIddictSecurityLogIdentityConsts.OpenIddict,
                        Action = IdentitySecurityLogActionConsts.ChangePassword,
                        UserName = request.Username ?? string.Empty,
                        ClientId = request.ClientId ?? string.Empty
                    });

                    if (changePasswordType == ChangePasswordType.ShouldChangePasswordOnNextLogin)
                    {
                        user.SetShouldChangePasswordOnNextLogin(false);
                    }

                    await UserManager.UpdateAsync(user);
                    return await SetSuccessResultAsync(request, user);
                }
                else
                {
                    Logger.LogInformation("ChangePassword failed for username: {username}, reason: {changePasswordResult}", request.Username, changePasswordResult.Errors.Select(x => x.Description).JoinAsString(", "));

                    var properties = new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = changePasswordResult.Errors.Select(x => x.Description).JoinAsString(", ")
                    });
                    return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                }
            }
            else
            {
                Logger.LogInformation("Authentication failed for username: {username}, reason: InvalidAuthenticatorCode", request.Username);

                var properties = new AuthenticationProperties(new Dictionary<string, string?>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Invalid authenticator code!"
                });

                return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }
        }
        else
        {
            Logger.LogInformation($"Authentication failed for username: {{{request.Username}}}, reason: {{{changePasswordType.ToString()}}}");

            await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
            {
                Identity = OpenIddictSecurityLogIdentityConsts.OpenIddict,
                Action = OpenIddictSecurityLogActionConsts.LoginNotAllowed,
                UserName = request.Username ?? string.Empty,
                ClientId = request.ClientId ?? string.Empty
            });

            var properties = new AuthenticationProperties(
                items: new Dictionary<string, string?>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = changePasswordType.ToString()
                },
                parameters: new Dictionary<string, object?>
                {
                    ["userId"] = user.Id.ToString("N"),
                    ["changePasswordToken"] = await UserManager.GenerateUserTokenAsync(user, TokenOptions.DefaultProvider, changePasswordType.ToString())
                });

            return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
    }

    protected virtual Task<IActionResult> HandleConfirmUserAsync(OpenIddictRequest request, IdentityUser user)
    {
        Logger.LogInformation($"{request.Username} needs to confirm email/phone number");

        var properties = new AuthenticationProperties(
            items: new Dictionary<string, string?>
            {
                [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = CenseqErrorDescriptionConsts.RequiresConfirmUser
            },
            parameters: new Dictionary<string, object?>
            {
                ["userId"] = user.Id.ToString("N"),
                ["email"] = user.Email ?? string.Empty,
                ["phoneNumber"] = user.PhoneNumber ?? string.Empty
            });

        return Task.FromResult<IActionResult>(Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme));
    }

    protected virtual async Task<IActionResult> SetSuccessResultAsync(OpenIddictRequest request, IdentityUser user)
    {
        // Clear the dynamic claims cache.
        await IdentityDynamicClaimsPrincipalContributorCache.ClearAsync(user.Id, user.TenantId);

        // Create a new ClaimsPrincipal containing the claims that
        // will be used to create an id_token, a token or a code.
        var principal = await SignInManager.CreateUserPrincipalAsync(user);

        // Create session and add SessionId claim
        var session = await CreateSessionAsync(user, request);
        if (session != null)
        {
            var sessionIdClaim = new Claim(AbpClaimTypes.SessionId, session.SessionId)
                .SetDestinations(OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken);
            principal.Identities.FirstOrDefault()!.AddClaim(sessionIdClaim);
        }

        var rememberMe = request.GetParameter("RememberMe")?.ToString() ?? string.Empty;
        if (!rememberMe.IsNullOrWhiteSpace() && bool.TryParse(rememberMe, out var rememberMeValue) && rememberMeValue)
        {
            var claim = new Claim(AbpClaimTypes.RememberMe, true.ToString()).SetDestinations(OpenIddictConstants.Destinations.AccessToken);
            principal.Identities.FirstOrDefault()!.AddClaim(claim);
        }

        principal.SetScopes(request.GetScopes());
        principal.SetResources(await GetResourcesAsync(request.GetScopes()));

        await OpenIddictClaimsPrincipalManager.HandleAsync(request, principal);

        await IdentitySecurityLogManager.SaveAsync(
            new IdentitySecurityLogContext
            {
                Identity = OpenIddictSecurityLogIdentityConsts.OpenIddict,
                Action = OpenIddictSecurityLogActionConsts.LoginSucceeded,
                UserName = request.Username ?? string.Empty,
                ClientId = request.ClientId ?? string.Empty
            }
        );

        return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    protected virtual async Task<IdentitySession?> CreateSessionAsync(IdentityUser user, OpenIddictRequest request)
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
            Logger.LogError(ex, "Failed to create session for user {UserId}", user.Id);
            return null;
        }
    }

    protected virtual string GetDeviceType()
    {
        var httpContext = HttpContext;
        if (httpContext == null)
        {
            return IdentitySessionDevices.Web;
        }

        var userAgent = httpContext.Request.Headers.UserAgent.ToString();
        if (string.IsNullOrEmpty(userAgent))
        {
            return IdentitySessionDevices.Web;
        }

        // 简单的设备类型检测
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
            var httpContext = HttpContext;
            if (httpContext == null)
            {
                return string.Empty;
            }

            var options = LazyServiceProvider.LazyGetRequiredService<IOptions<IdentitySessionOptions>>().Value;
            if (!options.SaveDeviceInfo)
            {
                return string.Empty;
            }

            var userAgent = httpContext.Request.Headers.UserAgent.ToString();
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
            var httpContext = HttpContext;
            if (httpContext == null)
            {
                return string.Empty;
            }

            var ips = new List<string>();

            // 尝试获取 X-Forwarded-For 头（经过代理时）
            var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(forwardedFor))
            {
                ips.AddRange(forwardedFor.Split(',').Select(ip => ip.Trim()).Where(ip => !string.IsNullOrWhiteSpace(ip)));
            }

            // 添加远程 IP
            var remoteIp = httpContext.Connection.RemoteIpAddress?.ToString();
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

    protected virtual async Task<bool> IsTfaEnabledAsync(IdentityUser user)
    {
        return UserManager.SupportsUserTwoFactor &&
               await UserManager.GetTwoFactorEnabledAsync(user) &&
               (await UserManager.GetValidTwoFactorProvidersAsync(user)).Count > 0;
    }

    public enum ChangePasswordType
    {
        ShouldChangePasswordOnNextLogin,
        PeriodicallyChangePassword
    }
}
