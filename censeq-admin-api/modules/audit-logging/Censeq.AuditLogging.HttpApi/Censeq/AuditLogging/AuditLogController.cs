using System;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace Censeq.AuditLogging;

/// <summary>
/// 审计日志控制器，提供对应的 HTTP API。
/// </summary>
[RemoteService(Name = "AuditLogging")]
[Area("auditLogging")]
[ControllerName("AuditLog")]
[Route("api/audit-logging/audit-logs")]
public class AuditLogController : AbpControllerBase, IAuditLogAppService
{
    /// <summary>
    /// AuditLogAppService。
    /// </summary>
    protected IAuditLogAppService AuditLogAppService { get; }

    /// <summary>
    /// 初始化 AuditLogController 实例。
    /// </summary>
    /// <param name="auditLogAppService">auditLogAppService。</param>
    public AuditLogController(IAuditLogAppService auditLogAppService)
    {
        AuditLogAppService = auditLogAppService;
    }

    /// <summary>
    /// 异步获取审计日志列表。
    /// </summary>
    /// <param name="input">查询输入。</param>
    /// <returns>审计日志列表。</returns>
    [HttpGet]
    public virtual Task<PagedResultDto<AuditLogDto>> GetListAsync([FromQuery] GetAuditLogsInput input)
    {
        return AuditLogAppService.GetListAsync(input);
    }

    /// <summary>
    /// 异步获取审计日志详情。
    /// </summary>
    /// <param name="id">标识。</param>
    /// <returns>审计日志详情。</returns>
    [HttpGet]
    [Route("{id}")]
    public virtual Task<AuditLogDto> GetAsync(Guid id)
    {
        return AuditLogAppService.GetAsync(id);
    }

    /// <summary>
    /// 异步删除审计日志。
    /// </summary>
    /// <param name="id">标识。</param>
    /// <returns>表示异步操作的任务。</returns>
    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return AuditLogAppService.DeleteAsync(id);
    }
}
