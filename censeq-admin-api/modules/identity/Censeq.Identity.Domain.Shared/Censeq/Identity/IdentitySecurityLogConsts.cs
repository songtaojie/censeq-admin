namespace Censeq.Identity;

/// <summary>
/// 身份安全日志常量定义
/// </summary>
public class IdentitySecurityLogConsts
{
    /// <summary>
    /// 最大应用名称长度，默认值：96
    /// </summary>
    public static int MaxApplicationNameLength { get; set; } = 96;

    /// <summary>
    /// 最大身份标识长度，默认值：96
    /// </summary>
    public static int MaxIdentityLength { get; set; } = 96;

    /// <summary>
    /// 最大动作名称长度，默认值：96
    /// </summary>
    public static int MaxActionLength { get; set; } = 96;

    /// <summary>
    /// 最大用户名长度，默认值：256
    /// </summary>
    public static int MaxUserNameLength { get; set; } = 256;

    /// <summary>
    /// 最大租户名称长度，默认值：64
    /// </summary>
    public static int MaxTenantNameLength { get; set; } = 64;

    /// <summary>
    /// 最大客户端IP地址长度，默认值：64
    /// </summary>
    public static int MaxClientIpAddressLength { get; set; } = 64;

    /// <summary>
    /// 最大客户端标识长度，默认值：64
    /// </summary>
    public static int MaxClientIdLength { get; set; } = 64;

    /// <summary>
    /// 最大关联标识长度，默认值：64
    /// </summary>
    public static int MaxCorrelationIdLength { get; set; } = 64;

    /// <summary>
    /// 最大浏览器信息长度，默认值：512
    /// </summary>
    public static int MaxBrowserInfoLength { get; set; } = 512;
}
