using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Censeq.Identity;

namespace Censeq.Account;

public interface IProfileAppService : IApplicationService
{
    Task<ProfileDto> GetAsync();

    Task<ProfileDto> UpdateAsync(UpdateProfileDto input);

    Task ChangePasswordAsync(ChangePasswordInput input);
}
