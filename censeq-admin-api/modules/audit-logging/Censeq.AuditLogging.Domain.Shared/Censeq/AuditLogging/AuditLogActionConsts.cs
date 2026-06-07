namespace Censeq.AuditLogging;

/// <summary>
/// 审计日志操作常量，集中声明常量。
/// </summary>
public class AuditLogActionConsts
{
    /// <summary>
    /// Default value: 256
    /// </summary>
    public static int MaxServiceNameLength { get; set; } = 256;

    /// <summary>
    /// Default value: 128
    /// </summary>
    public static int MaxMethodNameLength { get; set; } = 128;

    /// <summary>
    /// Default value: 2000
    /// </summary>
    public static int MaxParametersLength { get; set; } = 2000;
}
