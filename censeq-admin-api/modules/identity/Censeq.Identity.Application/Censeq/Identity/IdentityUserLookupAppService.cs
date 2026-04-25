using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Users;
using Censeq.Identity.Integration;

namespace Censeq.Identity;

[Obsolete("Use IdentityUserIntegrationService for module-to-module (or service-to-service) communication.")]
[Authorize(IdentityPermissions.UserLookup.Default)]
/// <summary>
/// 身份用户查找应用服务
/// </summary>
public class IdentityUserLookupAppService : IdentityAppServiceBase, IIdentityUserLookupAppService
{
    /// <summary>
    /// I身份用户集成服务
    /// </summary>
    protected IIdentityUserIntegrationService IdentityUserIntegrationService { get; }

    public IdentityUserLookupAppService(
        IIdentityUserIntegrationService identityUserIntegrationService)
    {
        IdentityUserIntegrationService = identityUserIntegrationService;
    }

    /// <summary>
    /// Task<UserData>
    /// </summary>
    public virtual async Task<UserData> FindByIdAsync(Guid id)
    {
        return await IdentityUserIntegrationService.FindByIdAsync(id);
    }

    /// <summary>
    /// Task<UserData>
    /// </summary>
    public virtual async Task<UserData> FindByUserNameAsync(string userName)
    {
        return await IdentityUserIntegrationService.FindByUserNameAsync(userName);
    }

    /// <summary>
    /// Task<List结果Dto<UserData>>
    /// </summary>
    public virtual async Task<ListResultDto<UserData>> SearchAsync(UserLookupSearchInputDto input)
    {
        return await IdentityUserIntegrationService.SearchAsync(input);
    }

    /// <summary>
    /// Task<long>
    /// </summary>
    public virtual async Task<long> GetCountAsync(UserLookupCountInputDto input)
    {
        return await IdentityUserIntegrationService.GetCountAsync(input);
    }
}
