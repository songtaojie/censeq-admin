using System.Collections.Generic;
using System.Threading.Tasks;
using Censeq.PermissionManagement.Entities;
using JetBrains.Annotations;

namespace Censeq.PermissionManagement;

/// <summary>
/// 权限管理器。
/// 提供权限授予状态的查询、设置和清理能力。
/// </summary>
public interface IPermissionManager
{
    /// <summary>
    /// 获取单个权限在指定提供者上的授予情况。
    /// </summary>
    /// <param name="permissionName">权限名称。</param>
    /// <param name="providerName">权限提供者名称。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    /// <returns>权限授予状态和授予来源。</returns>
    Task<PermissionWithGrantedProviders> GetAsync(string permissionName, string providerName, string providerKey);

    /// <summary>
    /// 批量获取权限在指定提供者上的授予情况。
    /// </summary>
    /// <param name="permissionNames">权限名称集合。</param>
    /// <param name="provideName">权限提供者名称。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    /// <returns>批量权限授予状态。</returns>
    Task<MultiplePermissionWithGrantedProviders> GetAsync(string[] permissionNames, string provideName, string providerKey);

    /// <summary>
    /// 获取当前提供者所有权限。
    /// </summary>
    /// <param name="providerName">权限提供者名称。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    /// <returns>全部权限授予状态。</returns>
    Task<List<PermissionWithGrantedProviders>> GetAllAsync([NotNull] string providerName, [NotNull] string providerKey);

    /// <summary>
    /// 设置权限授予状态。
    /// </summary>
    /// <param name="permissionName">权限名称。</param>
    /// <param name="providerName">权限提供者名称。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    /// <param name="isGranted">是否授予。</param>
    /// <returns>异步任务。</returns>
    Task SetAsync(string permissionName, string providerName, string providerKey, bool isGranted);

    /// <summary>
    /// 更新权限授予记录的提供者标识。
    /// </summary>
    /// <param name="permissionGrant">权限授予记录。</param>
    /// <param name="providerKey">新的提供者标识。</param>
    /// <returns>更新后的权限授予记录。</returns>
    Task<PermissionGrant> UpdateProviderKeyAsync(PermissionGrant permissionGrant, string providerKey);

    /// <summary>
    /// 删除指定提供者标识下的全部权限授予记录。
    /// </summary>
    /// <param name="providerName">权限提供者名称。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    /// <returns>异步任务。</returns>
    Task DeleteAsync(string providerName, string providerKey);
}
