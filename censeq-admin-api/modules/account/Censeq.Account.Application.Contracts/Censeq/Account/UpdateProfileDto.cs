using Volo.Abp.ObjectExtending;
using Volo.Abp.Validation;
using Censeq.Identity;

namespace Censeq.Account;

/// <summary>
/// 更新个人资料 DTO。
/// </summary>
public class UpdateProfileDto : ExtensibleObject
{
    /// <summary>
    /// 用户名。
    /// </summary>
    [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxUserNameLength))]
    public string? UserName { get; set; }

    /// <summary>
    /// 邮箱。
    /// </summary>
    [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxEmailLength))]
    public string? Email { get; set; }

    /// <summary>
    /// 名。
    /// </summary>
    [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxNameLength))]
    public string? Name { get; set; }

    /// <summary>
    /// 姓。
    /// </summary>
    [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxSurnameLength))]
    public string? Surname { get; set; }

    /// <summary>
    /// 手机号。
    /// </summary>
    [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPhoneNumberLength))]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// 头像地址。
    /// </summary>
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// 并发标记。
    /// </summary>
    public string? ConcurrencyStamp { get; set; }
}
