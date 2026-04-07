using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Censeq.Admin.Settings;
using Censeq.Identity;
using Volo.Abp.Identity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Settings;

namespace Censeq.Admin.Web.Pages.Account.Logout;

public class LogoutModel : AccountPageModel
{
    public IdentitySessionManager IdentitySessionManager { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ReturnUrlHash { get; set; }

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

        if (await SettingProvider.IsTrueAsync(AdminSettingNames.EnableLocalLogin))
        {
            return RedirectToPage("/Account/Login");
        }

        return RedirectToPage("/");
    }

    public virtual Task<IActionResult> OnPostAsync()
    {
        return Task.FromResult<IActionResult>(Page());
    }

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
