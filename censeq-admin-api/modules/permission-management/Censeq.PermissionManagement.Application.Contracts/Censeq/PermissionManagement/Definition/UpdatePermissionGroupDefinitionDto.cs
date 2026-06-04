using System.ComponentModel.DataAnnotations;

namespace Censeq.PermissionManagement;

/// <summary>更新权限组显示名称请求 DTO</summary>
public class UpdatePermissionGroupDefinitionDto
{
    /// <summary>
    /// 显示名称。
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string DisplayName { get; set; } = string.Empty;
}
