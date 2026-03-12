using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Censeq.PermissionManagement;
using Volo.Abp.Authorization.Permissions;

namespace Censeq.PermissionManagement;

/// <summary>
/// Ȩ�޶������л�
/// </summary>
public interface IPermissionDefinitionSerializer
{
    /// <summary>
    /// ���л�
    /// </summary>
    /// <param name="permissionGroups">Ȩ���鶨��</param>
    /// <returns>Ȩ���鶨���¼��Ȩ�޶����¼</returns>
    Task<(PermissionGroupDefinitionRecord[], PermissionDefinitionRecord[])>SerializeAsync(IEnumerable<PermissionGroupDefinition> permissionGroups);

    /// <summary>
    /// ���л�
    /// </summary>
    /// <param name="permissionGroup">Ȩ���鶨��</param>
    /// <returns>Ȩ���鶨���¼</returns>
    Task<PermissionGroupDefinitionRecord> SerializeAsync(PermissionGroupDefinition permissionGroup);

    /// <summary>
    /// ���л�
    /// </summary>
    /// <param name="permission">Ȩ�޶���</param>
    /// <param name="permissionGroup">Ȩ���鶨��</param>
    /// <returns>Ȩ�޶����¼</returns>
    Task<PermissionDefinitionRecord> SerializeAsync( PermissionDefinition permission, [CanBeNull] PermissionGroupDefinition? permissionGroup);
}