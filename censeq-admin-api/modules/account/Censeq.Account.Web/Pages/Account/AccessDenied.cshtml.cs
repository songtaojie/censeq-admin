using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Censeq.Account.Web.Pages.Account;

/// <summary>
/// 拒绝访问页面模型。
/// </summary>
public class AccessDeniedModel : AccountPageModel
{
    /// <summary>
    /// 返回地址。
    /// </summary>
    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    /// <summary>
    /// 返回地址哈希。
    /// </summary>
    [BindProperty(SupportsGet = true)]
    public string? ReturnUrlHash { get; set; }

    /// <summary>
    /// 异步处理页面 GET 请求。
    /// </summary>
    /// <returns>页面处理结果。</returns>
    public virtual Task<IActionResult> OnGetAsync()
    {
        return Task.FromResult<IActionResult>(Page());
    }

    /// <summary>
    /// 异步处理页面 POST 请求。
    /// </summary>
    /// <returns>页面处理结果。</returns>
    public virtual Task<IActionResult> OnPostAsync()
    {
        return Task.FromResult<IActionResult>(Page());
    }
}
