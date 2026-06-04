using Censeq.AuditLogging.Entities;

namespace Censeq.AuditLogging;

/// <summary>
/// 带用户名的实体变更信息。
/// </summary>
public class EntityChangeWithUsername
{
    /// <summary>
    /// 实体变更记录。
    /// </summary>
    public EntityChange EntityChange { get; set; }

    /// <summary>
    /// 用户名。
    /// </summary>
    public string? UserName { get; set; }
}
