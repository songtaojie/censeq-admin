using Censeq.Admin.Entities;

namespace Censeq.Admin.AuditLogging;

public class EntityChangeWithUsername
{
    public required EntityChange EntityChange { get; set; }

    public string? UserName { get; set; }
}
