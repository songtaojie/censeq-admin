using System.Threading.Tasks;
using Volo.Abp.UI.Navigation.Urls;

namespace Censeq.Account.Emailing;

/// <summary>
/// 账户应用地址提供者扩展方法。
/// </summary>
public static class AppUrlProviderAccountExtensions
{
    /// <summary>
    /// 异步获取密码重置地址。
    /// </summary>
    /// <param name="appUrlProvider">应用地址提供者。</param>
    /// <param name="appName">应用程序名称。</param>
    /// <returns>密码重置地址。</returns>
    public static Task<string> GetResetPasswordUrlAsync(this IAppUrlProvider appUrlProvider, string appName)
    {
        return appUrlProvider.GetUrlAsync(appName, AccountUrlNames.PasswordReset);
    }
}
