using System.Threading.Tasks;

namespace Censeq.OpenIddict;

public interface ICenseqOpenIddictClaimsPrincipalHandler
{
    Task HandleAsync(CenseqOpenIddictClaimsPrincipalHandlerContext context);
}
