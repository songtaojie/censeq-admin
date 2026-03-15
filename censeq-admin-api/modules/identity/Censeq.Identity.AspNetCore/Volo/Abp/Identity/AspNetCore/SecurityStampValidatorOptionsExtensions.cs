using Microsoft.AspNetCore.Identity;
using static Censeq.Identity.AspNetCore.CenseqSecurityStampValidatorCallback;

namespace Censeq.Identity.AspNetCore;

public static class SecurityStampValidatorOptionsExtensions
{
    public static SecurityStampValidatorOptions UpdatePrincipal(this SecurityStampValidatorOptions options, CenseqRefreshingPrincipalOptions CenseqRefreshingPrincipalOptions)
    {
        var previousOnRefreshingPrincipal = options.OnRefreshingPrincipal;
        options.OnRefreshingPrincipal = async context =>
        {
            await SecurityStampValidatorCallback.UpdatePrincipal(context, CenseqRefreshingPrincipalOptions);
            if(previousOnRefreshingPrincipal != null)
            {
                await previousOnRefreshingPrincipal.Invoke(context);
            }
        };
        return options;
    }
}
