using Censeq.PermissionManagement.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;

namespace Censeq.PermissionManagement;

/// <summary>
/// ��̬Ȩ�޶���洢���ڴ滺����
/// </summary>
public interface IDynamicPermissionDefinitionStoreInMemoryCache
{
    /// <summary>
    /// ������
    /// </summary>
    string CacheStamp { get; set; }

    /// <summary>
    /// ͬ���ź���
    /// </summary>
    SemaphoreSlim SyncSemaphore { get; }

    /// <summary>
    /// �����ʱ��
    /// </summary>
    DateTime? LastCheckTime { get; set; }

    /// <summary>
    /// ���Ȩ��
    /// </summary>
    /// <param name="permissionGroupRecords"></param>
    /// <param name="permissionRecords"></param>
    /// <returns></returns>
    Task FillAsync(List<PermissionGroup> permissionGroupRecords,List<PermissionDefinitionRecord> permissionRecords);

    /// <summary>
    /// ��ȡȨ��
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    PermissionDefinition? GetPermissionOrNull(string name);

    /// <summary>
    /// ��ȡ����Ȩ��
    /// </summary>
    /// <returns></returns>
    IReadOnlyList<PermissionDefinition> GetPermissions();

    /// <summary>
    /// ��ȡȨ���鶨��
    /// </summary>
    /// <returns></returns>
    IReadOnlyList<PermissionGroupDefinition> GetGroups();
}