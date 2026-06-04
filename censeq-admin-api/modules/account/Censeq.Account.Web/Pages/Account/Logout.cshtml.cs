using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Censeq.Account.Settings;
using Censeq.Identity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Settings;

namespace Censeq.Account.Web.Pages.Account;

/// <summary>
/// 退出登录页面模型。
/// </summary>
public class LogoutModel : AccountPageModel
{
    /// <summary>
    /// Identity 会话管理器。
    /// </summary>
    public IdentitySessionManager IdentitySessionManager { get; set; }

    /// <summary>
    /// 返回地址。
    /// </summary>
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    /// <summary>
    /// 返回地址哈希。
    /// </summary>
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string? ReturnUrlHash { get; set; }

    /// <summary>
    /// 异步处理页面 GET 请求。
    /// </summary>
    /// <returns>页面处理结果。</returns>
    public virtual async Task<IActionResult> OnGetAsync()
    {
        // 删除当前会话
        await DeleteCurrentSessionAsync();

        await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
        {
            Identity = IdentitySecurityLogIdentityConsts.Identity,
            Action = IdentitySecurityLogActionConsts.Logout
        });

        await SignInManager.SignOutAsync();
        if (ReturnUrl != null)
        {
            return await RedirectSafelyAsync(ReturnUrl, ReturnUrlHash);
        }

        if (await SettingProvider.IsTrueAsync(AccountSettingNames.EnableLocalLogin))
        {
            return RedirectToPage("/Account/Login");
        }

        return RedirectToPage("/");
    }

    /// <summary>
    /// 异步处理页面 POST 请求。
    /// </summary>
    /// <returns>页面处理结果。</returns>
    public virtual Task<IActionResult> OnPostAsync()
    {
        return Task.FromResult<IActionResult>(Page());
    }

    /// <summary>
    /// 异步删除当前会话。
    /// </summary>
    /// <returns>表示异步操作的任务。</returns>
    protected virtual async Task DeleteCurrentSessionAsync()
    {
        try
        {
            var sessionId = CurrentUser.FindClaim(AbpClaimTypes.SessionId)?.Value;
            if (!string.IsNullOrEmpty(sessionId))
            {
                await IdentitySessionManager.DeleteAsync(sessionId);
            }
        }
        catch
        {
            // 忽略删除会话时的错误，确保用户能够正常登出
        }
    }
}
