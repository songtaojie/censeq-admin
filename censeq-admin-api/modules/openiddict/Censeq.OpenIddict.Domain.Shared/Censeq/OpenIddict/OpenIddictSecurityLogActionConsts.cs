namespace Censeq.OpenIddict;

/// <summary>
/// OpenIddict安全日志操作常量，集中声明常量。
/// </summary>
public class OpenIddictSecurityLogActionConsts
{
    /// <summary>
    /// 登录成功结果。
    /// </summary>
    public static string LoginSucceeded { get; set; } = "LoginSucceeded";

    /// <summary>
    /// 账号锁定的登录结果。
    /// </summary>
    public static string LoginLockedout { get; set; } = "LoginLockedout";

    /// <summary>
    /// 不允许登录的登录结果。
    /// </summary>
    public static string LoginNotAllowed { get; set; } = "LoginNotAllowed";

    /// <summary>
    /// 需要双因素认证的登录结果。
    /// </summary>
    public static string LoginRequiresTwoFactor { get; set; } = "LoginRequiresTwoFactor";

    /// <summary>
    /// 登录失败结果。
    /// </summary>
    public static string LoginFailed { get; set; } = "LoginFailed";

    /// <summary>
    /// 用户名无效的登录结果。
    /// </summary>
    public static string LoginInvalidUserName { get; set; } = "LoginInvalidUserName";

    /// <summary>
    /// 用户名或密码无效的登录结果。
    /// </summary>
    public static string LoginInvalidUserNameOrPassword { get; set; } = "LoginInvalidUserNameOrPassword";
}