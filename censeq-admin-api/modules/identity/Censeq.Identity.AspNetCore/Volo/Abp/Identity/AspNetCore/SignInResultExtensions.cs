using Microsoft.AspNetCore.Identity;

namespace Censeq.Identity.AspNetCore;

/// <summary>
/// 登录结果扩展方法
/// </summary>
public static class SignInResultExtensions
{
    /// <summary>
    /// 将登录结果转换为安全日志操作类型
    /// </summary>
    /// <param name="result">登录结果</param>
    /// <returns>安全日志操作类型字符串</returns>
    public static string ToIdentitySecurityLogAction(this SignInResult result)
    {
        if (result.Succeeded)
        {
            return IdentitySecurityLogActionConsts.LoginSucceeded;
        }

        if (result.IsLockedOut)
        {
            return IdentitySecurityLogActionConsts.LoginLockedout;
        }

        if (result.RequiresTwoFactor)
        {
            return IdentitySecurityLogActionConsts.LoginRequiresTwoFactor;
        }

        if (result.IsNotAllowed)
        {
            return IdentitySecurityLogActionConsts.LoginNotAllowed;
        }

        if (!result.Succeeded)
        {
            return IdentitySecurityLogActionConsts.LoginFailed;
        }

        return IdentitySecurityLogActionConsts.LoginFailed;
    }
}
