using Censeq.AuditLogging.Entities;
using System.Threading.Tasks;
using Volo.Abp.Auditing;

namespace Censeq.AuditLogging;

public interface IAuditLogInfoToAuditLogConverter
{
    Task<AuditLog> ConvertAsync(AuditLogInfo auditLogInfo);
}
