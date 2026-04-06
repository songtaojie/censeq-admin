using System;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace Censeq.AuditLogging;

[RemoteService(Name = "AuditLogging")]
[Area("auditLogging")]
[ControllerName("AuditLog")]
[Route("api/audit-logging/audit-logs")]
public class AuditLogController : AbpControllerBase, IAuditLogAppService
{
    protected IAuditLogAppService AuditLogAppService { get; }

    public AuditLogController(IAuditLogAppService auditLogAppService)
    {
        AuditLogAppService = auditLogAppService;
    }

    [HttpGet]
    public virtual Task<PagedResultDto<AuditLogDto>> GetListAsync([FromQuery] GetAuditLogsInput input)
    {
        return AuditLogAppService.GetListAsync(input);
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<AuditLogDto> GetAsync(Guid id)
    {
        return AuditLogAppService.GetAsync(id);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return AuditLogAppService.DeleteAsync(id);
    }
}
