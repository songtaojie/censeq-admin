using System.Threading.Tasks;
using Volo.Abp.Data;

namespace Censeq.OpenIddict;

public interface IOpenIddictDbConcurrencyExceptionHandler
{
    Task HandleAsync(AbpDbConcurrencyException exception);
}
