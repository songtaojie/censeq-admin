using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Auditing;
using Censeq.Identity;
using Volo.Abp.Validation;

namespace Censeq.Account.Web.Pages.Account;

//TODO: Implement live password complexity check on the razor view!
/// <summary>
/// 重置密码页面模型。
/// </summary>
public class ResetPasswordModel : AccountPageModel
{
    /// <summary>
    /// 用户标识。
    /// </summary>
    [Required]
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid UserId { get; set; }

    /// <summary>
    /// 重置令牌。
    /// </summary>
    [Required]
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string? ResetToken { get; set; }

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
    /// 密码。
    /// </summary>
    [Required]
    [BindProperty]
    [DataType(DataType.Password)]
    [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPasswordLength))]
    [DisableAuditing]
    public string? Password { get; set; }

    /// <summary>
    /// 确认密码。
    /// </summary>
    [Required]
    [BindProperty]
    [DataType(DataType.Password)]
    [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPasswordLength))]
    [DisableAuditing]
    public string? ConfirmPassword { get; set; }

    /// <summary>
    /// 是否令牌无效。
    /// </summary>
    public bool InvalidToken { get; set; }

    /// <summary>
    /// 异步处理页面 GET 请求。
    /// </summary>
    /// <returns>页面处理结果。</returns>
    public virtual async Task<IActionResult> OnGetAsync()
    {
        ValidateModel();

        InvalidToken = !await AccountAppService.VerifyPasswordResetTokenAsync(
            new VerifyPasswordResetTokenInput
            {
                UserId = UserId,
                ResetToken = ResetToken!
            }
        );

        return Page();
    }

    /// <summary>
    /// 异步处理页面 POST 请求。
    /// </summary>
    /// <returns>页面处理结果。</returns>
    public virtual async Task<IActionResult> OnPostAsync()
    {
        try
        {
            ValidateModel();

            await AccountAppService.ResetPasswordAsync(
                new ResetPasswordDto
                {
                    UserId = UserId,
                    ResetToken = ResetToken!,
                    Password = Password!
                }
            );
        }
        catch (CenseqIdentityResultException e)
        {
            if (!string.IsNullOrWhiteSpace(e.Message))
            {
                Alerts.Warning(GetLocalizeExceptionMessage(e));
                return Page();
            }

            throw;
        }
        catch (AbpValidationException)
        {
            return Page();
        }

        //TODO: Try to automatically login!
        return RedirectToPage("./ResetPasswordConfirmation", new
        {
            returnUrl = ReturnUrl,
            returnUrlHash = ReturnUrlHash
        });
    }

    /// <summary>
    /// 验证页面模型。
    /// </summary>
    protected override void ValidateModel()
    {
        if (!Equals(Password, ConfirmPassword))
        {
            ModelState.AddModelError("ConfirmPassword", L["PasswordsDoNotMatch"]);
        }

        base.ValidateModel();
    }
}
