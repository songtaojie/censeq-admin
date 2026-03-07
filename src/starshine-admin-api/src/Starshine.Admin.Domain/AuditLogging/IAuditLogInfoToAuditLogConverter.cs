using Starshine.Admin.Entities;
using System.Threading.Tasks;
using Volo.Abp.Auditing;

namespace Starshine.Admin.AuditLogging;

public interface IAuditLogInfoToAuditLogConverter
{
    Task<AuditLog> ConvertAsync(AuditLogInfo auditLogInfo);
}
