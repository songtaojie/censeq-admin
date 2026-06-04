using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Censeq.PermissionManagement;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Authorization.Permissions;

namespace Censeq.PermissionManagement;

/// <summary>
/// 客户端权限管理器扩展方法。
/// </summary>
public static class ClientPermissionManagerExtensions
{
    /// <summary>
    /// 获取客户端标识。
    /// </summary>
    /// <param name="permissionManager">权限管理器。</param>
    /// <param name="clientId">客户端标识。</param>
    /// <param name="permissionName">权限Name。</param>
    /// <returns>异步操作结果。</returns>
    public static Task<PermissionWithGrantedProviders> GetForClientAsync([NotNull] this IPermissionManager permissionManager, string clientId, string permissionName)
    {
        Check.NotNull(permissionManager, nameof(permissionManager));

        return permissionManager.GetAsync(permissionName, ClientPermissionValueProvider.ProviderName, clientId);
    }

    /// <summary>
    /// 获取客户端的全部数据。
    /// </summary>
    /// <param name="permissionManager">权限管理器。</param>
    /// <param name="clientId">客户端标识。</param>
    /// <returns>异步操作结果。</returns>
    public static Task<List<PermissionWithGrantedProviders>> GetAllForClientAsync([NotNull] this IPermissionManager permissionManager, string clientId)
    {
        Check.NotNull(permissionManager, nameof(permissionManager));

        return permissionManager.GetAllAsync(ClientPermissionValueProvider.ProviderName, clientId);
    }

    /// <summary>
    /// 设置客户端标识。
    /// </summary>
    /// <param name="permissionManager">权限管理器。</param>
    /// <param name="clientId">客户端标识。</param>
    /// <param name="permissionName">权限Name。</param>
    /// <param name="isGranted">sGranted。</param>
    /// <returns>表示异步操作的任务。</returns>
    public static Task SetForClientAsync([NotNull] this IPermissionManager permissionManager, string clientId, [NotNull] string permissionName, bool isGranted)
    {
        Check.NotNull(permissionManager, nameof(permissionManager));

        return permissionManager.SetAsync(permissionName, ClientPermissionValueProvider.ProviderName, clientId, isGranted);
    }
}
