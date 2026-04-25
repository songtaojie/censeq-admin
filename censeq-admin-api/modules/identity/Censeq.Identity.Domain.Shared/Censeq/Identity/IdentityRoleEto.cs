using System;
using Volo.Abp.Auditing;
using Volo.Abp.MultiTenancy;

namespace Censeq.Identity;

/// <summary>
/// 身份角色事件传输对象（ETO）
/// </summary>
[Serializable]
/// <summary>
/// 身份角色事件传输对象
/// </summary>
public class IdentityRoleEto : IMultiTenant, IHasEntityVersion
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
    /// 角色名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 是否为默认角色
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// 是否为静态角色
    /// </summary>
    public bool IsStatic { get; set; }

    /// <summary>
    /// 是否为公开角色
    /// </summary>
    public bool IsPublic { get; set; }

    /// <summary>
    /// 实体版本号
    /// </summary>
    public int EntityVersion { get; set; }
}
