using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.MultiTenancy;

namespace Censeq.Identity;

/// <summary>
/// 登录日志 DTO
/// </summary>
public class IdentitySecurityLogDto : EntityDto<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    public string? TenantName { get; set; }

    public string? ApplicationName { get; set; }

    /// <summary>
    /// 身份来源（Identity / IdentityExternal）
    /// </summary>
    public string? Identity { get; set; }

    /// <summary>
    /// 操作（LoginSucceeded / LoginFailed.* / Logout）
    /// </summary>
    public string Action { get; set; } = default!;

    public Guid? UserId { get; set; }

    public string? UserName { get; set; }

    public string? ClientId { get; set; }

    public string? CorrelationId { get; set; }

    public string? ClientIpAddress { get; set; }

    public string? BrowserInfo { get; set; }

    public DateTime CreationTime { get; set; }
}