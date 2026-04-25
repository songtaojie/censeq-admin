namespace Censeq.Identity;

/// <summary>
/// 身份用户登录常量定义
/// </summary>
public static class IdentityUserLoginConsts
{
    /// <summary>
    /// 最大登录提供程序长度，默认值：64
    /// </summary>
    public static int MaxLoginProviderLength { get; set; } = 64;

    /// <summary>
    /// 最大提供程序键长度，默认值：196
    /// </summary>
    public static int MaxProviderKeyLength { get; set; } = 196;

    /// <summary>
    /// 最大提供程序显示名称长度，默认值：128
    /// </summary>
    public static int MaxProviderDisplayNameLength { get; set; } = 128;
}
