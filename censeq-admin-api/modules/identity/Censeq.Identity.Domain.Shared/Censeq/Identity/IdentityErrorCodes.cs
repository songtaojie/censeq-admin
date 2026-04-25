namespace Censeq.Identity;

/// <summary>
/// 身份模块错误码常量
/// </summary>
public static class IdentityErrorCodes
{
    /// <summary>
    /// 用户不能删除自己
    /// </summary>
    public const string UserSelfDeletion = "Censeq.Identity:010001";

    /// <summary>
    /// 超出最大允许的组织单元成员数
    /// </summary>
    public const string MaxAllowedOuMembership = "Censeq.Identity:010002";

    /// <summary>
    /// 外部用户不能更改密码
    /// </summary>
    public const string ExternalUserPasswordChange = "Censeq.Identity:010003";

    /// <summary>
    /// 组织单元显示名称重复
    /// </summary>
    public const string DuplicateOrganizationUnitDisplayName = "Censeq.Identity:010004";

    /// <summary>
    /// 静态角色不能重命名
    /// </summary>
    public const string StaticRoleRenaming = "Censeq.Identity:010005";

    /// <summary>
    /// 静态角色不能删除
    /// </summary>
    public const string StaticRoleDeletion = "Censeq.Identity:010006";

    /// <summary>
    /// 用户不能更改双因素认证
    /// </summary>
    public const string UsersCanNotChangeTwoFactor = "Censeq.Identity:010007";

    /// <summary>
    /// 不能更改双因素认证
    /// </summary>
    public const string CanNotChangeTwoFactor = "Censeq.Identity:010008";

    /// <summary>
    /// 不能委托给自己
    /// </summary>
    public const string YouCannotDelegateYourself = "Censeq.Identity:010009";

    /// <summary>
    /// 声明名称已存在
    /// </summary>
    public const string ClaimNameExist = "Censeq.Identity:010021";
}
