namespace Censeq.AuditLogging;

/// <summary>
/// 实体属性变更常量，集中声明常量。
/// </summary>
public class EntityPropertyChangeConsts
{
    /// <summary>
    /// Default value: 512
    /// </summary>
    public static int MaxNewValueLength { get; set; } = 512;

    /// <summary>
    /// Default value: 512
    /// </summary>
    public static int MaxOriginalValueLength { get; set; } = 512;

    /// <summary>
    /// Default value: 128
    /// </summary>
    public static int MaxPropertyNameLength { get; set; } = 128;

    /// <summary>
    /// Default value: 64
    /// </summary>
    public static int MaxPropertyTypeFullNameLength { get; set; } = 64;
}
