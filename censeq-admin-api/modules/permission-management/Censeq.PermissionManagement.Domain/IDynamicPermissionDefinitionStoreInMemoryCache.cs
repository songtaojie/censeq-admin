using Censeq.PermissionManagement.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;

namespace Censeq.PermissionManagement;

/// <summary>
/// 动态权限定义存储的进程内缓存。
/// 保存从数据库还原出的权限组和权限定义对象。
/// </summary>
public interface IDynamicPermissionDefinitionStoreInMemoryCache
{
    /// <summary>
    /// 缓存标记。
    /// </summary>
    string CacheStamp { get; set; }

    /// <summary>
    /// 同步信号量，用于串行化缓存刷新。
    /// </summary>
    SemaphoreSlim SyncSemaphore { get; }

    /// <summary>
    /// 最后检查缓存标记的时间。
    /// </summary>
    DateTime? LastCheckTime { get; set; }

    /// <summary>
    /// 使用数据库记录填充进程内权限定义缓存。
    /// </summary>
    /// <param name="permissionGroupRecords">权限组记录。</param>
    /// <param name="permissionRecords">权限定义记录。</param>
    /// <returns>异步任务。</returns>
    Task FillAsync(List<PermissionGroup> permissionGroupRecords,List<PermissionDefinitionRecord> permissionRecords);

    /// <summary>
    /// 根据名称获取权限定义。
    /// </summary>
    /// <param name="name">权限名称。</param>
    /// <returns>权限定义不存在时返回 null。</returns>
    PermissionDefinition? GetPermissionOrNull(string name);

    /// <summary>
    /// 获取全部权限定义。
    /// </summary>
    /// <returns>权限定义列表。</returns>
    IReadOnlyList<PermissionDefinition> GetPermissions();

    /// <summary>
    /// 获取全部权限组定义。
    /// </summary>
    /// <returns>权限组定义列表。</returns>
    IReadOnlyList<PermissionGroupDefinition> GetGroups();
}
