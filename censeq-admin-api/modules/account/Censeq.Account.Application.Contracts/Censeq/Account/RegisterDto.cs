using System.ComponentModel.DataAnnotations;
using Volo.Abp.Auditing;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Validation;
using Censeq.Identity;

namespace Censeq.Account;

/// <summary>
/// 注册 DTO。
/// </summary>
public class RegisterDto : ExtensibleObject
{
    /// <summary>
    /// 用户名。
    /// </summary>
    [Required]
    [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxUserNameLength))]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 邮箱地址。
    /// </summary>
    [Required]
    [EmailAddress]
    [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxEmailLength))]
    public string EmailAddress { get; set; } = string.Empty;

    /// <summary>
    /// 密码。
    /// </summary>
    [Required]
    [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPasswordLength))]
    [DataType(DataType.Password)]
    [DisableAuditing]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 应用程序名称。
    /// </summary>
    [Required]
    public string AppName { get; set; } = string.Empty;
}
