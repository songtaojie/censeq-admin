using System.ComponentModel.DataAnnotations;

namespace Censeq.Identity;

/// <summary>
/// 更新用户角色 DTO
/// </summary>
public class IdentityUserUpdateRolesDto
{
    /// <summary>
    /// 角色名称列表
    /// </summary>
    [Required]
    public string[] RoleNames { get; set; }
}
