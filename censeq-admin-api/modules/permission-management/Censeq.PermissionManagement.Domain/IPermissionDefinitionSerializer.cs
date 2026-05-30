using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp.Authorization.Permissions;
using Censeq.PermissionManagement.Entities;

namespace Censeq.PermissionManagement;

/// <summary>
/// 权限定义序列化器。
/// 将 ABP 运行时权限定义转换为可持久化的权限组和权限定义记录。
/// </summary>
public interface IPermissionDefinitionSerializer
{
    /// <summary>
    /// 序列化多个权限组定义。
    /// </summary>
    /// <param name="permissionGroups">权限组定义集合。</param>
    /// <returns>权限组记录和权限定义记录。</returns>
    Task<(PermissionGroup[], PermissionDefinitionRecord[])>SerializeAsync(IEnumerable<PermissionGroupDefinition> permissionGroups);

    /// <summary>
    /// 序列化单个权限组定义。
    /// </summary>
    /// <param name="permissionGroup">权限组定义。</param>
    /// <returns>权限组记录。</returns>
    Task<PermissionGroup> SerializeAsync(PermissionGroupDefinition permissionGroup);

    /// <summary>
    /// 序列化单个权限定义。
    /// </summary>
    /// <param name="permission">权限定义。</param>
    /// <param name="permissionGroup">所属权限组定义。</param>
    /// <returns>权限定义记录。</returns>
    Task<PermissionDefinitionRecord> SerializeAsync( PermissionDefinition permission, [CanBeNull] PermissionGroupDefinition? permissionGroup);
}
