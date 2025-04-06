using System.Threading.Tasks;
using Volo.Abp.UI.Navigation.Urls;

namespace Starshine.Admin.Account.Emailing;

public static class AppUrlProviderAccountExtensions
{
    public static Task<string> GetResetPasswordUrlAsync(this IAppUrlProvider appUrlProvider, string appName)
    {
        return appUrlProvider.GetUrlAsync(appName, AdminConsts.PasswordReset);
    }
}
