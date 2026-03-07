using Starshine.Admin.Entities;

namespace Starshine.Admin.AuditLogging;

public class EntityChangeWithUsername
{
    public required EntityChange EntityChange { get; set; }

    public string? UserName { get; set; }
}
