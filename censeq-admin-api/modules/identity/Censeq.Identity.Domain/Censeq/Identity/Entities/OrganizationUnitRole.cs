using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Censeq.Identity.Entities;

/// <summary>
/// 表示角色与组织单元之间的关联。
/// </summary>
public class OrganizationUnitRole : CreationAuditedEntity, IMultiTenant
{
    /// <summary>
    /// 此实体的租户标识。
    /// </summary>
    public virtual Guid? TenantId { get; protected set; }

    /// <summary>
    /// 角色的标识。
    /// </summary>
    public virtual Guid RoleId { get; protected set; }

    /// <summary>
    /// 关联 <see cref="OrganizationUnit"/> 的标识。
    /// </summary>
    public virtual Guid OrganizationUnitId { get; protected set; }

    /// <summary>
    /// 初始化 <see cref="OrganizationUnitRole"/> 类的新实例。
    /// </summary>
    protected OrganizationUnitRole()
    {

    }

    /// <summary>
    /// 初始化 <see cref="OrganizationUnitRole"/> 类的新实例。
    /// </summary>
    /// <param name="tenantId">租户标识</param>
    /// <param name="roleId">角色的标识。</param>
    /// <param name="organizationUnitId">关联 <see cref="OrganizationUnit"/> 的标识。</param>
    public OrganizationUnitRole(Guid roleId, Guid organizationUnitId, Guid? tenantId = null)
    {
        RoleId = roleId;
        OrganizationUnitId = organizationUnitId;
        TenantId = tenantId;
    }

    /// <summary>
    /// 获取实体的复合主键。
    /// </summary>
    /// <returns>复合主键数组。</returns>
    public override object[] GetKeys()
    {
        return new object[] { OrganizationUnitId, RoleId };
    }
}
