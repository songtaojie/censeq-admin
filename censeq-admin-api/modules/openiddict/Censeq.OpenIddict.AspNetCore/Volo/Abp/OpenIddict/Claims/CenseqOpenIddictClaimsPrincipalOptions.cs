using Volo.Abp.Collections;

namespace Censeq.OpenIddict;

public class CenseqOpenIddictClaimsPrincipalOptions
{
    public ITypeList<ICenseqOpenIddictClaimsPrincipalHandler> ClaimsPrincipalHandlers { get; }

    public CenseqOpenIddictClaimsPrincipalOptions()
    {
        ClaimsPrincipalHandlers = new TypeList<ICenseqOpenIddictClaimsPrincipalHandler>();
    }
}
