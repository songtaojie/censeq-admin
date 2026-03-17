using System.ComponentModel.DataAnnotations;
using Volo.Abp.Validation;
using Censeq.Identity;

namespace Censeq.Account;

public class SendPasswordResetCodeDto
{
    [Required]
    [EmailAddress]
    [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxEmailLength))]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string AppName { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }

    public string? ReturnUrlHash { get; set; }
}
