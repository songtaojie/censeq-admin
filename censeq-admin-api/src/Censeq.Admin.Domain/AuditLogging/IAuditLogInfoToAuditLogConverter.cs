using Censeq.Admin.Entities;
using System.Threading.Tasks;
using Volo.Abp.Auditing;

namespace Censeq.Admin.AuditLogging;

public interface IAuditLogInfoToAuditLogConverter
{
    Task<AuditLog> ConvertAsync(AuditLogInfo auditLogInfo);
}
