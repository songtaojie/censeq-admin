using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Authorization.Permissions;

namespace Censeq.PermissionManagement;

/// <summary>
/// 用户权限管理器扩展
/// </summary>
public static class UserPermissionManagerExtensions
{
    /// <summary>
    /// Task<List<PermissionWithGrantedProviders>>
    /// </summary>
    public static Task<List<PermissionWithGrantedProviders>> GetAllForUserAsync([NotNull] this IPermissionManager permissionManager, Guid userId)
    {
        Check.NotNull(permissionManager, nameof(permissionManager));

        return permissionManager.GetAllAsync(UserPermissionValueProvider.ProviderName, userId.ToString());
    }

    public static Task SetForUserAsync([NotNull] this IPermissionManager permissionManager, Guid userId, [NotNull] string name, bool isGranted)
    {
        Check.NotNull(permissionManager, nameof(permissionManager));

        return permissionManager.SetAsync(name, UserPermissionValueProvider.ProviderName, userId.ToString(), isGranted);
    }
}
