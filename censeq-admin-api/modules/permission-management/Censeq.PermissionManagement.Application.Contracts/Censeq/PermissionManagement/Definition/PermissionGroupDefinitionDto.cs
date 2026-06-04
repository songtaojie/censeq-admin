namespace Censeq.PermissionManagement;

/// <summary>权限组定义 DTO</summary>
public class PermissionGroupDefinitionDto
{
    /// <summary>
    /// 标识。
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// 权限组名称。
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// 显示名称。
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;
}
