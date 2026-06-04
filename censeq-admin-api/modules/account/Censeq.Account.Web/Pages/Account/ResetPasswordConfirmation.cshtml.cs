using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Censeq.Account.Web.Pages.Account;

/// <summary>
/// 重置密码确认页面模型。
/// </summary>
[AllowAnonymous]
public class ResetPasswordConfirmationModel : AccountPageModel
{
    /// <summary>
    /// 返回地址。
    /// </summary>
    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; set; } = "/";

    /// <summary>
    /// 返回地址哈希。
    /// </summary>
    [BindProperty(SupportsGet = true)]
    public string? ReturnUrlHash { get; set; }

    /// <summary>
    /// 异步处理页面 GET 请求。
    /// </summary>
    /// <returns>页面处理结果。</returns>
    public virtual async Task<IActionResult> OnGetAsync()
    {
        ReturnUrl = await GetRedirectUrlAsync(ReturnUrl, ReturnUrlHash);

        return Page();
    }
}
