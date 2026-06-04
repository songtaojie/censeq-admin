using Censeq.AuditLogging.Entities;
using System.Threading.Tasks;
using Volo.Abp.Auditing;

namespace Censeq.AuditLogging;

/// <summary>
/// 审计日志信息转换器接口。
/// </summary>
public interface IAuditLogInfoToAuditLogConverter
{
    /// <summary>
    /// 异步转换审计日志信息。
    /// </summary>
    /// <param name="auditLogInfo">审计日志信息。</param>
    /// <returns>审计日志实体。</returns>
    Task<AuditLog> ConvertAsync(AuditLogInfo auditLogInfo);
}
