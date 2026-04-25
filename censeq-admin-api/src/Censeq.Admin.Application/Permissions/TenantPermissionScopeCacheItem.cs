namespace Censeq.PermissionManagement;

/// <summary>
/// 缓存项：某个租户被平台授权的权限名称集合
/// </summary>
[Serializable]
public class TenantPermissionScopeCacheItem
{
    public HashSet<string> AllowedPermissions { get; set; } = new(StringComparer.Ordinal);
}
