namespace Censeq.AuditLogging.ObjectExtending;

/// <summary>
/// 审计日志模块扩展常量，集中声明常量。
/// </summary>
public static class AuditLoggingModuleExtensionConsts
{
    /// <summary>
    /// 模块名称。
    /// </summary>
    public const string ModuleName = "AuditLogging";

    /// <summary>
    /// 实体名称常量。
    /// </summary>
    public static class EntityNames
    {
        /// <summary>
        /// 审计日志实体名称。
        /// </summary>
        public const string AuditLog = "AuditLog";

        /// <summary>
        /// 审计日志操作实体名称。
        /// </summary>
        public const string AuditLogAction = "AuditLogAction";

        /// <summary>
        /// 实体变更实体名称。
        /// </summary>
        public const string EntityChange = "EntityChange";
    }
}
