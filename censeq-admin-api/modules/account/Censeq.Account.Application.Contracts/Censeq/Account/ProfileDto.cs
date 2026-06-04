using Volo.Abp.Domain.Entities;
using Volo.Abp.ObjectExtending;

namespace Censeq.Account;

/// <summary>
/// 个人资料 DTO。
/// </summary>
public class ProfileDto : ExtensibleObject, IHasConcurrencyStamp
{
    /// <summary>
    /// 用户名。
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 邮箱。
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 名。
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 姓。
    /// </summary>
    public string Surname { get; set; } = string.Empty;

    /// <summary>
    /// 手机号。
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// 头像地址。
    /// </summary>
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// 是否外部用户。
    /// </summary>
    public bool IsExternal { get; set; }

    /// <summary>
    /// 是否已有密码。
    /// </summary>
    public bool HasPassword { get; set; }

    /// <summary>
    /// 并发标记。
    /// </summary>
    public string ConcurrencyStamp { get; set; } = string.Empty;
}
