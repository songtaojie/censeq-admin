using System.Threading.Tasks;

namespace Censeq.Abp.OpenIddict;

public interface IAbpOpenIddictClaimsPrincipalHandler
{
    Task HandleAsync(AbpOpenIddictClaimsPrincipalHandlerContext context);
}
