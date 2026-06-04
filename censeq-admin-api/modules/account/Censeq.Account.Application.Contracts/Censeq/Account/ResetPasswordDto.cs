using System.ComponentModel.DataAnnotations;
using Volo.Abp.Auditing;

namespace Censeq.Account;

/// <summary>
/// 重置密码 DTO。
/// </summary>
public class ResetPasswordDto
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

    /// <summary>
    /// 密码。
    /// </summary>
    [Required]
    [DisableAuditing]
    public string Password { get; set; } = string.Empty;
}
