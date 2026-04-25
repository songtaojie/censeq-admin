namespace Censeq.Identity;

/// <summary>
/// 身份角色常量定义
/// </summary>
public static class IdentityRoleConsts
{
    /// <summary>
    /// 最大名称长度，默认值：256
    /// </summary>
    public static int MaxNameLength { get; set; } = 256;

    /// <summary>
    /// 最大代码长度，默认值：64
    /// </summary>
    public static int MaxCodeLength { get; set; } = 64;

    /// <summary>
    /// 最大标准化名称长度，默认值：256
    /// </summary>
    public static int MaxNormalizedNameLength { get; set; } = 256;
}
