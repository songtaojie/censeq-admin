using System.ComponentModel.DataAnnotations;

namespace Censeq.PermissionManagement;

/// <summary>更新权限项显示名称请求 DTO</summary>
public class UpdatePermissionDefinitionDto
{
    [Required]
    [MaxLength(256)]
    public string DisplayName { get; set; } = string.Empty;
}
