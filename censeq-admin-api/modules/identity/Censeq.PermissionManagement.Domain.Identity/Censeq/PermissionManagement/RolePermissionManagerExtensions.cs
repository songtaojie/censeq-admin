using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Authorization.Permissions;

namespace Censeq.PermissionManagement;

/// <summary>
/// 角色权限管理器扩展
/// </summary>
public static class RolePermissionManagerExtensions
{
    /// <summary>
    /// Task<PermissionWithGrantedProviders>
    /// </summary>
    public static Task<PermissionWithGrantedProviders> GetForRoleAsync([NotNull] this IPermissionManager permissionManager, string roleId, string permissionName)
    {
        Check.NotNull(permissionManager, nameof(permissionManager));

        return permissionManager.GetAsync(permissionName, RolePermissionValueProvider.ProviderName, roleId);
    }

    /// <summary>
    /// Task<List<PermissionWithGrantedProviders>>
    /// </summary>
    public static Task<List<PermissionWithGrantedProviders>> GetAllForRoleAsync([NotNull] this IPermissionManager permissionManager, string roleId)
    {
        Check.NotNull(permissionManager, nameof(permissionManager));

        return permissionManager.GetAllAsync(RolePermissionValueProvider.ProviderName, roleId);
    }

    public static Task SetForRoleAsync([NotNull] this IPermissionManager permissionManager, string roleId, [NotNull] string permissionName, bool isGranted)
    {
        Check.NotNull(permissionManager, nameof(permissionManager));

        return permissionManager.SetAsync(permissionName, RolePermissionValueProvider.ProviderName, roleId, isGranted);
    }
}
