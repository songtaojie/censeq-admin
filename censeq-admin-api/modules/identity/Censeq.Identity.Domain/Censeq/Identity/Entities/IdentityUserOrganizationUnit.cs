using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Censeq.Identity.Entities;

/// <summary>
/// 表示用户与组织单元的成员关系。
/// </summary>
public class IdentityUserOrganizationUnit : CreationAuditedEntity, IMultiTenant
{
    /// <summary>
    /// 此实体的租户标识。
    /// </summary>
    public virtual Guid? TenantId { get; protected set; }

    /// <summary>
    /// 用户的标识。
    /// </summary>
    public virtual Guid UserId { get; protected set; }

    /// <summary>
    /// 关联 <see cref="OrganizationUnit"/> 的标识。
    /// </summary>
    public virtual Guid OrganizationUnitId { get; protected set; }

    protected IdentityUserOrganizationUnit()
    {

    }

    public IdentityUserOrganizationUnit(Guid userId, Guid organizationUnitId, Guid? tenantId = null)
    {
        UserId = userId;
        OrganizationUnitId = organizationUnitId;
        TenantId = tenantId;
    }

    /// <summary>
    /// 获取实体的复合主键。
    /// </summary>
    /// <returns>复合主键数组。</returns>
    public override object[] GetKeys()
    {
        return new object[] { UserId, OrganizationUnitId };
    }
}
