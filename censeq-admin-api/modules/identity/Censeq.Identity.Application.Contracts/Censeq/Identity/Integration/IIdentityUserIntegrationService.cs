using Censeq.Identity.Application.Contracts.Censeq.Identity;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Users;

namespace Censeq.Identity.Application.Contracts.Censeq.Identity.Integration;

[IntegrationService]
public interface IIdentityUserIntegrationService : IApplicationService
{
    Task<string[]> GetRoleNamesAsync(Guid id);
    
    Task<UserData> FindByIdAsync(Guid id);

    Task<UserData> FindByUserNameAsync(string userName);

    Task<ListResultDto<UserData>> SearchAsync(UserLookupSearchInputDto input);

    Task<long> GetCountAsync(UserLookupCountInputDto input);
}