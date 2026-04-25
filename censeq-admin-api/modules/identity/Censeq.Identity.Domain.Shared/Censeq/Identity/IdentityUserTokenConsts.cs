namespace Censeq.Identity;

/// <summary>
/// 身份用户令牌常量定义
/// </summary>
public static class IdentityUserTokenConsts
{
    /// <summary>
    /// 最大登录提供程序长度，默认值：64
    /// </summary>
    public static int MaxLoginProviderLength { get; set; } = 64;

    /// <summary>
    /// 最大名称长度，默认值：128
    /// </summary>
    public static int MaxNameLength { get; set; } = 128;
}
