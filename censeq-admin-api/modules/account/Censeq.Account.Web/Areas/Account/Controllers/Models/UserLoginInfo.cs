using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Auditing;

namespace Censeq.Account.Web.Areas.Account.Controllers.Models;

/// <summary>
/// 用户登录信息。
/// </summary>
public class UserLoginInfo
{
    /// <summary>
    /// 用户名或邮箱地址。
    /// </summary>
    [Required]
    [StringLength(255)]
    public string? UserNameOrEmailAddress { get; set; }

    /// <summary>
    /// 密码。
    /// </summary>
    [Required]
    [StringLength(32)]
    [DataType(DataType.Password)]
    [DisableAuditing]
    public string? Password { get; set; }

    /// <summary>
    /// 是否记住登录。
    /// </summary>
    public bool RememberMe { get; set; }
}
