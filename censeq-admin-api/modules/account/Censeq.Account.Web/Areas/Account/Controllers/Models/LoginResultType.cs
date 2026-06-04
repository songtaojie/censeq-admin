namespace Censeq.Account.Web.Areas.Account.Controllers.Models;

/// <summary>
/// 登录结果类型。
/// </summary>
public enum LoginResultType : byte
{
    Success = 1,

    InvalidUserNameOrPassword = 2,

    NotAllowed = 3,

    LockedOut = 4,

    RequiresTwoFactor = 5
}
