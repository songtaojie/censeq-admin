using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Server.AspNetCore;

namespace Censeq.OpenIddict.Controllers;

/// <summary>
/// 登出控制器，提供对应的 HTTP API。
/// </summary>
[Route("connect/logout")]
[ApiExplorerSettings(IgnoreApi = true)]
public class LogoutController : CenseqOpenIddictControllerBase
{
    /// <summary>
    /// 获取指定标识的数据。
    /// </summary>
    /// <returns>查询结果。</returns>
    [HttpGet]
    public virtual async Task<IActionResult> GetAsync()
    {
        // Ask ASP.NET Core Identity to delete the local and external cookies created
        // when the user agent is redirected from the external identity provider
        // after a successful authentication flow (e.g Google or Facebook).
        await SignInManager.SignOutAsync();

        // Returning a SignOutResult will ask OpenIddict to redirect the user agent
        // to the post_logout_redirect_uri specified by the client application or to
        // the RedirectUri specified in the authentication properties if none was set.
        return SignOut(authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
}
