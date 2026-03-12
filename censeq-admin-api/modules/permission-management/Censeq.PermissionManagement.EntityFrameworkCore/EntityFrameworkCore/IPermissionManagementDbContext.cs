using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Censeq.PermissionManagement.Entities;

namespace Censeq.PermissionManagement.EntityFrameworkCore;

[ConnectionStringName(CenseqPermissionManagementDbProperties.ConnectionStringName)]
public interface IPermissionManagementDbContext : IEfCoreDbContext
{
    DbSet<PermissionGroupDefinitionRecord> PermissionGroups { get; }
    DbSet<PermissionDefinitionRecord> Permissions { get; }
    DbSet<PermissionGrant> PermissionGrants { get; }
}
