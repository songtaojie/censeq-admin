using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Censeq.PermissionManagement.Entities;

namespace Censeq.PermissionManagement.EntityFrameworkCore;

/// <summary>
/// 权限管理 DbContext 契约。
/// 暴露权限组、权限定义和权限授予的 DbSet。
/// </summary>
[ConnectionStringName(CenseqPermissionManagementDbProperties.ConnectionStringName)]
public interface IPermissionManagementDbContext : IEfCoreDbContext
{
    /// <summary>
    /// 权限组记录。
    /// </summary>
    DbSet<PermissionGroup> PermissionGroups { get; }

    /// <summary>
    /// 权限定义记录。
    /// </summary>
    DbSet<PermissionDefinitionRecord> Permissions { get; }

    /// <summary>
    /// 权限授予记录。
    /// </summary>
    DbSet<PermissionGrant> PermissionGrants { get; }
}
