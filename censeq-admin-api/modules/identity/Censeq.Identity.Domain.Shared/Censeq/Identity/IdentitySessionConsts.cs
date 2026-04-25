namespace Censeq.Identity;

/// <summary>
/// 身份会话常量定义
/// </summary>
public class IdentitySessionConsts
{
    /// <summary>
    /// 最大会话标识长度，默认值：128
    /// </summary>
    public static int MaxSessionIdLength { get; set; } = 128;

    /// <summary>
    /// 最大设备类型长度，默认值：64
    /// </summary>
    public static int MaxDeviceLength { get; set; } = 64;

    /// <summary>
    /// 最大设备信息长度，默认值：64
    /// </summary>
    public static int MaxDeviceInfoLength { get; set; } = 64;

    /// <summary>
    /// 最大客户端标识长度，默认值：64
    /// </summary>
    public static int MaxClientIdLength { get; set; } = 64;

    /// <summary>
    /// 最大IP地址长度，默认值：256
    /// </summary>
    public static int MaxIpAddressesLength { get; set; } = 256;
}
