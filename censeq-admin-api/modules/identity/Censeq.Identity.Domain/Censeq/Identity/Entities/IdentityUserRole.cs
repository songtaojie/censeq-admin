using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Censeq.Identity.Entities;

/// <summary>
/// 用户与角色的关联
/// </summary>
public class IdentityUserRole : Entity, IMultiTenant
{
    public virtual Guid? TenantId { get; protected set; }

    /// <summary>
    /// 关联用户的主键
    /// </summary>
    public virtual Guid UserId { get; protected set; }

    /// <summary>
    /// 关联角色的主键
    /// </summary>
    public virtual Guid RoleId { get; protected set; }

    protected IdentityUserRole()
    {

    }

    protected internal IdentityUserRole(Guid userId, Guid roleId, Guid? tenantId)
    {
        UserId = userId;
        RoleId = roleId;
        TenantId = tenantId;
    }

    public override object[] GetKeys()
    {
        return new object[] { UserId, RoleId };
    }
}
