namespace Censeq.PermissionManagement;

/// <summary>权限项定义 DTO</summary>
public class PermissionDefinitionDto
{
    public Guid Id { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ParentName { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
}
