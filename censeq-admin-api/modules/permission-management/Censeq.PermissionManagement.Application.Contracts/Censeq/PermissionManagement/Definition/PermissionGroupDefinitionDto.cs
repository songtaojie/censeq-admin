namespace Censeq.PermissionManagement;

/// <summary>权限组定义 DTO</summary>
public class PermissionGroupDefinitionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}
