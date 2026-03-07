namespace Starshine.Admin.Web.Areas.Account.Controllers.Models;

public enum UserLoginType : byte
{
    /// <summary>
    /// 登录成功
    /// </summary>
    Success = 1,

    /// <summary>
    /// 用户名或密码错误
    /// </summary>
    InvalidUserNameOrPassword = 2,

    /// <summary>
    /// 登录被禁止
    /// </summary>
    NotAllowed = 3,

    /// <summary>
    /// 锁定
    /// </summary>
    LockedOut = 4,

    /// <summary>
    /// 需要双因素认证
    /// </summary>
    RequiresTwoFactor = 5
}
