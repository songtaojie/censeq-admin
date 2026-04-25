namespace Censeq.Identity;

/// <summary>
/// 身份安全日志动作常量定义
/// </summary>
public class IdentitySecurityLogActionConsts
{
    /// <summary>
    /// 登录成功
    /// </summary>
    public static string LoginSucceeded { get; set; } = "LoginSucceeded";

    /// <summary>
    /// 登录被锁定
    /// </summary>
    public static string LoginLockedout { get; set; } = "LoginLockedout";

    /// <summary>
    /// 登录不被允许
    /// </summary>
    public static string LoginNotAllowed { get; set; } = "LoginNotAllowed";

    /// <summary>
    /// 登录需要双因素认证
    /// </summary>
    public static string LoginRequiresTwoFactor { get; set; } = "LoginRequiresTwoFactor";

    /// <summary>
    /// 登录失败
    /// </summary>
    public static string LoginFailed { get; set; } = "LoginFailed";

    /// <summary>
    /// 用户名无效
    /// </summary>
    public static string LoginInvalidUserName { get; set; } = "LoginInvalidUserName";

    /// <summary>
    /// 用户名或密码无效
    /// </summary>
    public static string LoginInvalidUserNameOrPassword { get; set; } = "LoginInvalidUserNameOrPassword";

    /// <summary>
    /// 登出
    /// </summary>
    public static string Logout { get; set; } = "Logout";

    /// <summary>
    /// 更改用户名
    /// </summary>
    public static string ChangeUserName { get; set; } = "ChangeUserName";

    /// <summary>
    /// 更改邮箱
    /// </summary>
    public static string ChangeEmail { get; set; } = "ChangeEmail";

    /// <summary>
    /// 更改电话号码
    /// </summary>
    public static string ChangePhoneNumber { get; set; } = "ChangePhoneNumber";

    /// <summary>
    /// 更改密码
    /// </summary>
    public static string ChangePassword { get; set; } = "ChangePassword";

    /// <summary>
    /// 启用双因素认证
    /// </summary>
    public static string TwoFactorEnabled { get; set; } = "TwoFactorEnabled";

    /// <summary>
    /// 禁用双因素认证
    /// </summary>
    public static string TwoFactorDisabled { get; set; } = "TwoFactorDisabled";
}
