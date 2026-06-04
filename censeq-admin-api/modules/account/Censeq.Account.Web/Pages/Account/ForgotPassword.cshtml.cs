using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Censeq.Account.Web.Consts;
using Censeq.Identity;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.Validation;

namespace Censeq.Account.Web.Pages.Account;

/// <summary>
/// 忘记密码页面模型。
/// </summary>
public class ForgotPasswordModel : AccountPageModel
{
    /// <summary>
    /// 邮箱。
    /// </summary>
    [Required]
    [EmailAddress]
    [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxEmailLength))]
    [BindProperty]
    [DisplayName("邮箱")]
    public string? Email { get; set; }

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
    public virtual Task<IActionResult> OnGetAsync()
    {
        return Task.FromResult<IActionResult>(Page());
    }

    /// <summary>
    /// 异步处理页面 POST 请求。
    /// </summary>
    /// <returns>页面处理结果。</returns>
    public virtual async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(Email))
        {
            return Page();
        }

        try
        {
            var AppUrlProvider = LazyServiceProvider.GetRequiredService<IAppUrlProvider>();
            var url = await AppUrlProvider.GetUrlAsync(CenseqAccountConsts.AppName, CenseqAccountConsts.PasswordReset);
            await AccountAppService.SendPasswordResetCodeAsync(
                new SendPasswordResetCodeDto
                {
                    Email = Email!,
                    AppName = CenseqAccountConsts.AppName, //TODO: Const!
                    ReturnUrl = ReturnUrl,
                    ReturnUrlHash = ReturnUrlHash
                }
            );
        }
        catch (UserFriendlyException e)
        {
            Alerts.Danger(GetLocalizeExceptionMessage(e));
            return Page();
        }


        return RedirectToPage(
            "./PasswordResetLinkSent",
            new
            {
                returnUrl = ReturnUrl,
                returnUrlHash = ReturnUrlHash
            });
    }
}
