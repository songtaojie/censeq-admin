using System;
using Volo.Abp.Application.Dtos;

namespace Censeq.AuditLogging;

/// <summary>
/// 获取审计日志列表输入
/// </summary>
public class GetAuditLogsInput : PagedAndSortedResultRequestDto
{
    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// HTTP方法(GET/POST等)
    /// </summary>
    public string? HttpMethod { get; set; }

    /// <summary>
    /// URL
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 应用名称
    /// </summary>
    public string? ApplicationName { get; set; }

    /// <summary>
    /// 客户端IP地址
    /// </summary>
    public string? ClientIpAddress { get; set; }

    /// <summary>
    /// 是否有异常
    /// </summary>
    public bool? HasException { get; set; }

    /// <summary>
    /// 最小执行时长(毫秒)
    /// </summary>
    public int? MinExecutionDuration { get; set; }

    /// <summary>
    /// 最大执行时长(毫秒)
    /// </summary>
    public int? MaxExecutionDuration { get; set; }
}
