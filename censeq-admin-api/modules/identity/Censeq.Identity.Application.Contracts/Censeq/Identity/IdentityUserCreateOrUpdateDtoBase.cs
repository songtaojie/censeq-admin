using System;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Validation;

namespace Censeq.Identity;

/// <summary>
/// 用户创建或更新 DTO 基类
/// </summary>
public abstract class IdentityUserCreateOrUpdateDtoBase : ExtensibleObject
{
    /// <summary>
    /// 用户名
    /// </summary>
    [Required]
    [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxUserNameLength))]
    public string UserName { get; set; }

    /// <summary>
    /// 名字
    /// </summary>
    [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxNameLength))]
    public string? Name { get; set; }

    /// <summary>
    /// 姓氏
    /// </summary>
    [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxSurnameLength))]
    public string? Surname { get; set; }

    /// <summary>
    /// 电子邮箱
    /// </summary>
    [Required]
    [EmailAddress]
    [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxEmailLength))]
    public string Email { get; set; }

    /// <summary>
    /// 电话号码
    /// </summary>
    [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPhoneNumberLength))]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// 是否激活
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// 是否启用锁定
    /// </summary>
    public bool LockoutEnabled { get; set; }

    /// <summary>
    /// 角色名称列表
    /// </summary>
    [CanBeNull]
    public string[] RoleNames { get; set; }

    /// <summary>
    /// 构造函数
    /// </summary>
    protected IdentityUserCreateOrUpdateDtoBase() : base(false)
    {

    }
}
