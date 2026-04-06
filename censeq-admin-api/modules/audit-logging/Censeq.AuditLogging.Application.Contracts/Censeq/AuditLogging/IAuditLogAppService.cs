using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Censeq.AuditLogging;

/// <summary>
/// 审计日志应用服务接口
/// </summary>
public interface IAuditLogAppService : IApplicationService
{
    /// <summary>
    /// 获取审计日志列表
    /// </summary>
    Task<PagedResultDto<AuditLogDto>> GetListAsync(GetAuditLogsInput input);

    /// <summary>
    /// 获取审计日志详情
    /// </summary>
    Task<AuditLogDto> GetAsync(Guid id);

    /// <summary>
    /// 删除审计日志
    /// </summary>
    Task DeleteAsync(Guid id);
}
