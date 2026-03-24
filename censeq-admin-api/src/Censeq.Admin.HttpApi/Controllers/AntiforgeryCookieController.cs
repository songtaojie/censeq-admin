using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;

namespace Censeq.Admin.Controllers;

/// <summary>
/// 跨域 SPA 在首次写操作前调用：下发 XSRF-TOKEN（可读）与 .AspNetCore.Antiforgery（HttpOnly）成对 Cookie。
/// </summary>
[Area("app")]
[Route("api/app")]
public class AntiforgeryCookieController : AbpController
{
    protected IAbpAntiForgeryManager AntiForgeryManager { get; }

    public AntiforgeryCookieController(IAbpAntiForgeryManager antiForgeryManager)
    {
        AntiForgeryManager = antiForgeryManager;
    }

    [HttpGet("antiforgery")]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public virtual IActionResult Refresh()
    {
        AntiForgeryManager.SetCookie();
        return Ok();
    }
}
