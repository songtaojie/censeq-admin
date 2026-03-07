using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Censeq.Admin.Dtos;
using Volo.Abp.Auditing;
using Volo.Abp.Identity;
using Volo.Abp.Validation;

namespace Censeq.Admin.Web.Pages.Account.Password;

//TODO: Implement live password complexity check on the razor view!
public class ResetPasswordModel : AccountPageModel
{
    [Required]
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid? UserId { get; set; }

    [Required]
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ResetToken { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ReturnUrlHash { get; set; }

    [Required]
    [BindProperty]
    [DataType(DataType.Password)]
    [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPasswordLength))]
    [DisableAuditing]
    public string Password { get; set; }

    [Required]
    [BindProperty]
    [DataType(DataType.Password)]
    [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPasswordLength))]
    [DisableAuditing]
    public string ConfirmPassword { get; set; }

    public bool InvalidToken { get; set; }

    public virtual async Task<IActionResult> OnGetAsync()
    {
        ValidateModel();

        InvalidToken = !await AccountAppService.VerifyPasswordResetTokenAsync(
            new VerifyPasswordResetTokenInput
            {
                UserId = UserId.Value,
                ResetToken = ResetToken
            }
        );

        return Page();
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
        try
        {
            ValidateModel();

            await AccountAppService.ResetPasswordAsync(
                new ResetPasswordInput
                {
                    UserId = UserId.Value,
                    ResetToken = ResetToken,
                    Password = Password
                }
            );
        }
        catch (AbpIdentityResultException e)
        {
            if (!string.IsNullOrWhiteSpace(e.Message))
            {
                ModelState.AddModelError("SubmitError", GetLocalizeExceptionMessage(e));
                return Page();
            }
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

    protected override void ValidateModel()
    {
        if (!Equals(Password, ConfirmPassword))
        {
            ModelState.AddModelError("ConfirmPassword",
                L["'{0}' and '{1}' do not match.", "ConfirmPassword", "Password"]);
        }

        base.ValidateModel();
    }
}
