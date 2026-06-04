namespace Censeq.PermissionManagement;

/// <summary>
/// 权限定义记录约束常量。
/// </summary>
public class PermissionDefinitionRecordConsts
{
    /// <summary>
    /// MaxNameLength。
    /// </summary>
    public static int MaxNameLength { get; set; } = 128;
    /// <summary>
    /// MaxDisplayNameLength。
    /// </summary>
    public static int MaxDisplayNameLength { get; set; } = 256;
    /// <summary>
    /// MaxLocalizationKeyLength。
    /// </summary>
    public static int MaxLocalizationKeyLength { get; set; } = 512;
    /// <summary>
    /// MaxProvidersLength。
    /// </summary>
    public static int MaxProvidersLength { get; set; } = 128;
    /// <summary>
    /// MaxStateCheckersLength。
    /// </summary>
    public static int MaxStateCheckersLength { get; set; } = 256;
}
