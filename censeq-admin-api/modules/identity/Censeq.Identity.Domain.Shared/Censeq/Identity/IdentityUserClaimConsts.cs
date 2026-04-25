namespace Censeq.Identity;

/// <summary>
/// 身份用户声明常量定义
/// </summary>
public static class IdentityUserClaimConsts
{
    /// <summary>
    /// 最大声明类型长度，默认值：256
    /// </summary>
    public static int MaxClaimTypeLength { get; set; } = 256;

    /// <summary>
    /// 最大声明值长度，默认值：1024
    /// </summary>
    public static int MaxClaimValueLength { get; set; } = 1024;
}
