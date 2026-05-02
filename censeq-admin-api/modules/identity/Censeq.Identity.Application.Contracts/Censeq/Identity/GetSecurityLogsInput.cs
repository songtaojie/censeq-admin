using System;
using Volo.Abp.Application.Dtos;

namespace Censeq.Identity;

/// <summary>
/// 查询登录日志输入
/// </summary>
public class GetSecurityLogsInput : PagedAndSortedResultRequestDto
{
    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 操作类型（如 LoginSucceeded / LoginFailed.* / Logout）
    /// </summary>
    public string? Action { get; set; }

    public string? UserName { get; set; }

    public string? ClientIpAddress { get; set; }
}
