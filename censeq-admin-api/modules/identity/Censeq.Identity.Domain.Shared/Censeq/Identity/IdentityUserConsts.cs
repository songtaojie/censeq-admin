using Volo.Abp.Users;

namespace Censeq.Identity;

/// <summary>
/// 身份用户常量定义
/// </summary>
public static class IdentityUserConsts
{
    /// <summary>
    /// 最大用户名长度
    /// </summary>
    public static int MaxUserNameLength { get; set; } = AbpUserConsts.MaxUserNameLength;

    /// <summary>
    /// 最大名称长度
    /// </summary>
    public static int MaxNameLength { get; set; } = AbpUserConsts.MaxNameLength;

    /// <summary>
    /// 最大姓氏长度
    /// </summary>
    public static int MaxSurnameLength { get; set; } = AbpUserConsts.MaxSurnameLength;

    /// <summary>
    /// 最大标准化用户名长度
    /// </summary>
    public static int MaxNormalizedUserNameLength { get; set; } = MaxUserNameLength;

    /// <summary>
    /// 最大邮箱长度
    /// </summary>
    public static int MaxEmailLength { get; set; } = AbpUserConsts.MaxEmailLength;

    /// <summary>
    /// 最大标准化邮箱长度
    /// </summary>
    public static int MaxNormalizedEmailLength { get; set; } = MaxEmailLength;

    /// <summary>
    /// 最大电话号码长度
    /// </summary>
    public static int MaxPhoneNumberLength { get; set; } = AbpUserConsts.MaxPhoneNumberLength;

    /// <summary>
    /// 最大密码长度，默认值：128
    /// </summary>
    public static int MaxPasswordLength { get; set; } = 128;

    /// <summary>
    /// 最大密码哈希长度，默认值：256
    /// </summary>
    public static int MaxPasswordHashLength { get; set; } = 256;

    /// <summary>
    /// 最大安全戳长度，默认值：256
    /// </summary>
    public static int MaxSecurityStampLength { get; set; } = 256;

    /// <summary>
    /// 最大登录提供程序长度，默认值：16
    /// </summary>
    public static int MaxLoginProviderLength { get; set; } = 16;
}
