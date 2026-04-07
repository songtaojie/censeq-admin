using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.MultiTenancy;

namespace Censeq.Identity;

/// <summary>
/// 用户会话 DTO
/// </summary>
public class IdentitySessionDto : EntityDto<Guid>, IMultiTenant
{
    /// <summary>
    /// 会话 ID
    /// </summary>
    public string SessionId { get; set; }

    /// <summary>
    /// 租户 ID
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// 用户 ID
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 设备类型（Web、Mobile、OAuth）
    /// </summary>
    public string Device { get; set; }

    /// <summary>
    /// 设备信息
    /// </summary>
    public string DeviceInfo { get; set; }

    /// <summary>
    /// 客户端 ID
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    /// IP 地址列表
    /// </summary>
    public string IpAddresses { get; set; }

    /// <summary>
    /// 登录时间
    /// </summary>
    public DateTime SignedIn { get; set; }

    /// <summary>
    /// 最后访问时间
    /// </summary>
    public DateTime? LastAccessed { get; set; }

    /// <summary>
    /// 是否当前会话
    /// </summary>
    public bool IsCurrentSession { get; set; }
}
