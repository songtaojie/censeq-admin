namespace Censeq.Identity;

/// <summary>
/// 身份安全日志身份标识常量定义
/// </summary>
public static class IdentitySecurityLogIdentityConsts
{
    /// <summary>
    /// 身份认证
    /// </summary>
    public static string Identity { get; set; } = "Identity";

    /// <summary>
    /// 外部身份认证
    /// </summary>
    public static string IdentityExternal { get; set; } = "IdentityExternal";

    /// <summary>
    /// 双因素身份认证
    /// </summary>
    public static string IdentityTwoFactor { get; set; } = "IdentityTwoFactor";
}
