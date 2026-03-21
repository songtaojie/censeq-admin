using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Censeq.Account;

public interface IDynamicClaimsAppService : IApplicationService
{
    Task RefreshAsync();
}
