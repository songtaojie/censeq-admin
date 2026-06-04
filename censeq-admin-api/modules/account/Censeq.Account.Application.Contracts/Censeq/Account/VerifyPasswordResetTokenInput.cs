using System.ComponentModel.DataAnnotations;

namespace Censeq.Account;

/// <summary>
/// 验证密码重置令牌输入。
/// </summary>
public class VerifyPasswordResetTokenInput
{
    /// <summary>
    /// 用户标识。
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 重置令牌。
    /// </summary>
    [Required]
    public string ResetToken { get; set; } = string.Empty;
}
