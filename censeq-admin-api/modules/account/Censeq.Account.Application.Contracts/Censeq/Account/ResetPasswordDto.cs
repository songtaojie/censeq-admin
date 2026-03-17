using System.ComponentModel.DataAnnotations;
using Volo.Abp.Auditing;

namespace Censeq.Account;

public class ResetPasswordDto
{
    public Guid UserId { get; set; }

    [Required]
    public string ResetToken { get; set; } = string.Empty;

    [Required]
    [DisableAuditing]
    public string Password { get; set; } = string.Empty;
}
