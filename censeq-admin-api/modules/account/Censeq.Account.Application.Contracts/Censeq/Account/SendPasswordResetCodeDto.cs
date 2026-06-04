using System.ComponentModel.DataAnnotations;
using Volo.Abp.Validation;
using Censeq.Identity;

namespace Censeq.Account;

/// <summary>
/// 发送密码重置码 DTO。
/// </summary>
public class SendPasswordResetCodeDto
{
    /// <summary>
    /// 邮箱。
    /// </summary>
    [Required]
    [EmailAddress]
    [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxEmailLength))]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 应用程序名称。
    /// </summary>
    [Required]
    public string AppName { get; set; } = string.Empty;

    /// <summary>
    /// 返回地址。
    /// </summary>
    public string? ReturnUrl { get; set; }

    /// <summary>
    /// 返回地址哈希。
    /// </summary>
    public string? ReturnUrlHash { get; set; }
}
