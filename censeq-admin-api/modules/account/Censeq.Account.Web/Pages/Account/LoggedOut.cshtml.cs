using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NUglify.Helpers;

namespace Censeq.Account.Web.Pages.Account;

/// <summary>
/// 已退出页面模型。
/// </summary>
public class LoggedOutModel : AccountPageModel
{
    /// <summary>
    /// 客户端名称。
    /// </summary>
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string? ClientName { get; set; }

    /// <summary>
    /// 登出 iframe 地址。
    /// </summary>
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string? SignOutIframeUrl { get; set; }

    /// <summary>
    /// 登出后重定向地址。
    /// </summary>
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string? PostLogoutRedirectUri { get; set; }

    /// <summary>
    /// 异步处理页面 GET 请求。
    /// </summary>
    /// <returns>页面处理结果。</returns>
    public virtual async Task<IActionResult> OnGetAsync()
    {
        await NormalizeUrlAsync();
        return Page();
    }

    /// <summary>
    /// 异步处理页面 POST 请求。
    /// </summary>
    /// <returns>页面处理结果。</returns>
    public virtual async Task<IActionResult> OnPostAsync()
    {
        await NormalizeUrlAsync();
        return Page();
    }
    
    /// <summary>
    /// 异步规范化地址。
    /// </summary>
    /// <returns>规范化后的地址。</returns>
    protected virtual async Task NormalizeUrlAsync()
    {
        if (!string.IsNullOrWhiteSpace(PostLogoutRedirectUri))
        {
            PostLogoutRedirectUri = Url.Content(await GetRedirectUrlAsync(PostLogoutRedirectUri));
        }
        
        if(!string.IsNullOrWhiteSpace(SignOutIframeUrl))
        {
            SignOutIframeUrl = Url.Content(await GetRedirectUrlAsync(SignOutIframeUrl));
        }
    }
}
