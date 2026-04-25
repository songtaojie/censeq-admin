using System.ComponentModel.DataAnnotations;

namespace Censeq.Admin;

/// <summary>
/// 重置租户管理员密码请求体
/// </summary>
public class ResetTenantAdminPasswordDto
{
    [Required]
    [MinLength(6)]
    [MaxLength(128)]
    public string NewPassword { get; set; } = null!;
}
