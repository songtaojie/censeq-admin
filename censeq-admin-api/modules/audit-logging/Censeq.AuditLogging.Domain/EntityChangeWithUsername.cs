using Censeq.AuditLogging.Entities;

namespace Censeq.AuditLogging;

public class EntityChangeWithUsername
{
    public EntityChange EntityChange { get; set; }

    public string? UserName { get; set; }
}
