using Microsoft.AspNetCore.Identity;

namespace Censeq.OpenIddict;

/// <summary>
/// SignIn 结果扩展方法。
/// </summary>
public static class CenseqSignInResultExtensions
{
    /// <summary>
    /// 转换为身份实体安全日志操作。
    /// </summary>
    /// <param name="result">结果。</param>
    /// <returns>操作结果。</returns>
    public static string ToIdentitySecurityLogAction(this SignInResult result)
    {
        if (result.Succeeded)
        {
            return OpenIddictSecurityLogActionConsts.LoginSucceeded;
        }

        if (result.IsLockedOut)
        {
            return OpenIddictSecurityLogActionConsts.LoginLockedout;
        }

        if (result.RequiresTwoFactor)
        {
            return OpenIddictSecurityLogActionConsts.LoginRequiresTwoFactor;
        }

        if (result.IsNotAllowed)
        {
            return OpenIddictSecurityLogActionConsts.LoginNotAllowed;
        }

        if (!result.Succeeded)
        {
            return OpenIddictSecurityLogActionConsts.LoginFailed;
        }

        return OpenIddictSecurityLogActionConsts.LoginFailed;
    }
}
