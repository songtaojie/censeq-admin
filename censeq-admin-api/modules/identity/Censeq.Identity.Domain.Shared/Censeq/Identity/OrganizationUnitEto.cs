using System;
using Volo.Abp.Auditing;
using Volo.Abp.MultiTenancy;

namespace Censeq.Identity;

/// <summary>
/// 组织单元事件传输对象（ETO）
/// </summary>
[Serializable]
/// <summary>
/// 组织单元事件传输对象
/// </summary>
public class OrganizationUnitEto : IMultiTenant, IHasEntityVersion
{
    /// <summary>
    /// 组织单元标识
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 租户标识
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// 父级组织单元标识
    /// </summary>
    public virtual Guid? ParentId { get; set; }

    /// <summary>
    /// 组织单元代码
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// 显示名称
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// 实体版本号
    /// </summary>
    public int EntityVersion { get; set; }
}
