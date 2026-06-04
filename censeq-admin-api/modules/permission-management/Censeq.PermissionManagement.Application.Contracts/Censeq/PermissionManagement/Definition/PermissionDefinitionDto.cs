namespace Censeq.PermissionManagement;

/// <summary>权限项定义 DTO</summary>
public class PermissionDefinitionDto
{
    /// <summary>
    /// 标识。
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// 权限组名称。
    /// </summary>
    public string GroupName { get; set; } = string.Empty;
    /// <summary>
    /// 权限名称。
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// 父级权限名称。
    /// </summary>
    public string? ParentName { get; set; }
    /// <summary>
    /// 显示名称。
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;
    /// <summary>
    /// 是否启用。
    /// </summary>
    public bool IsEnabled { get; set; }
}
