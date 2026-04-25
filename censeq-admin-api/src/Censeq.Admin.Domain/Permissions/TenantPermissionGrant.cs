using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Censeq.Admin.Permissions;

/// <summary>
/// 平台向租户授予的权限范围记录。
/// 租户内用户只能使用该表中存在的权限，超出范围的权限即便角色已授予也不生效。
/// </summary>
public class TenantPermissionGrant : CreationAuditedEntity<Guid>
{
    /// <summary>租户 ID（NOT NULL，此表仅存储租户级授权记录）</summary>
    public virtual Guid TenantId { get; protected set; }

    /// <summary>权限名称，如 CenseqIdentity.Users</summary>
    public virtual string PermissionName { get; protected set; } = null!;

    protected TenantPermissionGrant() { }

    public TenantPermissionGrant(Guid id, Guid tenantId, string permissionName)
        : base(id)
    {
        TenantId = tenantId;
        PermissionName = permissionName;
    }
}
