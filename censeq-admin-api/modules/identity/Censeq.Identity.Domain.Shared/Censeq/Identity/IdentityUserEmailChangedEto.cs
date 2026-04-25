using System;
using Volo.Abp.MultiTenancy;

namespace Censeq.Identity;

/// <summary>
/// 身份用户邮箱变更事件传输对象（ETO）
/// </summary>
[Serializable]
/// <summary>
/// 身份用户邮箱Changed事件传输对象
/// </summary>
public class IdentityUserEmailChangedEto : IMultiTenant
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
    /// 新邮箱地址
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// 旧邮箱地址
    /// </summary>
    public string OldEmail { get; set; }
}