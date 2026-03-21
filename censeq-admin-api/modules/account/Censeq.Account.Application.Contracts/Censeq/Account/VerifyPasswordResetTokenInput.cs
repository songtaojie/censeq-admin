using System.ComponentModel.DataAnnotations;

namespace Censeq.Account;

public class VerifyPasswordResetTokenInput
{
    public Guid UserId { get; set; }

    [Required]
    public string ResetToken { get; set; } = string.Empty;
}
