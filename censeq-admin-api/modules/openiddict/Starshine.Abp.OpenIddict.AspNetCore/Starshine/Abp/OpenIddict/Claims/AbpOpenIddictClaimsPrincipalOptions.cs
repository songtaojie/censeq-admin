using Volo.Abp.Collections;

namespace Censeq.Abp.OpenIddict;

public class AbpOpenIddictClaimsPrincipalOptions
{
    public ITypeList<IAbpOpenIddictClaimsPrincipalHandler> ClaimsPrincipalHandlers { get; }

    public AbpOpenIddictClaimsPrincipalOptions()
    {
        ClaimsPrincipalHandlers = new TypeList<IAbpOpenIddictClaimsPrincipalHandler>();
    }
}
