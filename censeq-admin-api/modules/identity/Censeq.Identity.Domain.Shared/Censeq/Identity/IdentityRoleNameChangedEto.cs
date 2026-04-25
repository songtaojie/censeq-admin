using System;
using Volo.Abp.MultiTenancy;

namespace Censeq.Identity;

/// <summary>
/// 身份角色名称变更事件传输对象（ETO）
/// </summary>
[Serializable]
/// <summary>
/// 身份角色名称Changed事件传输对象
/// </summary>
public class IdentityRoleNameChangedEto : IMultiTenant
{
    /// <summary>
    /// 角色标识
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 租户标识
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// 新角色名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 旧角色名称
    /// </summary>
    public string OldName { get; set; }
}
