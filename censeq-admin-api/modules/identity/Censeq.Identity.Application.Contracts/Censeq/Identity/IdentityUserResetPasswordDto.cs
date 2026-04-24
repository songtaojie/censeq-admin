using System.ComponentModel.DataAnnotations;

namespace Censeq.Identity;

/// <summary>
/// 管理员重置用户密码 DTO
/// </summary>
public class IdentityUserResetPasswordDto
{
    /// <summary>
    /// 新密码
    /// </summary>
    [Required]
    [MinLength(6)]
    public string NewPassword { get; set; } = string.Empty;
}
