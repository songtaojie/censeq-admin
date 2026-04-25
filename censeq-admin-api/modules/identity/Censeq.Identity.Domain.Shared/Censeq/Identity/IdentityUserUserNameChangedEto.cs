using System;
using Volo.Abp.MultiTenancy;

namespace Censeq.Identity;

/// <summary>
/// 身份用户名变更事件传输对象（ETO）
/// </summary>
[Serializable]
/// <summary>
/// 身份用户名称Changed事件传输对象
/// </summary>
public class IdentityUserUserNameChangedEto : IMultiTenant
{
    /// <summary>
    /// 用户标识
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 租户标识
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// 新用户名
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// 旧用户名
    /// </summary>
    public string OldUserName { get; set; }
}
