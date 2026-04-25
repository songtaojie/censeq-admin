using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Users;
using Censeq.Identity.Entities;

namespace Censeq.Identity.Integration;

/// <summary>
/// 身份用户集成服务
/// </summary>
public class IdentityUserIntegrationService : IdentityAppServiceBase, IIdentityUserIntegrationService
{
    /// <summary>
    /// I用户角色Finder
    /// </summary>
    protected IUserRoleFinder UserRoleFinder { get; }
    /// <summary>
    /// 身份用户仓储外部用户查找服务提供程序
    /// </summary>
    protected IdentityUserRepositoryExternalUserLookupServiceProvider UserLookupServiceProvider { get; }

    public IdentityUserIntegrationService(
        IUserRoleFinder userRoleFinder,
        IdentityUserRepositoryExternalUserLookupServiceProvider userLookupServiceProvider)
    {
        UserRoleFinder = userRoleFinder;
        UserLookupServiceProvider = userLookupServiceProvider;
    }

    /// <summary>
    /// Task<string[]>
    /// </summary>
    public virtual async Task<string[]> GetRoleNamesAsync(Guid id)
    {
        return await UserRoleFinder.GetRoleNamesAsync(id);
    }

    /// <summary>
    /// Task<UserData>
    /// </summary>
    public virtual async Task<UserData> FindByIdAsync(Guid id)
    {
        var userData = await UserLookupServiceProvider.FindByIdAsync(id);
        if (userData == null)
        {
            throw new EntityNotFoundException(typeof(IdentityUser), id);
        }

        return new UserData(userData);
    }

    /// <summary>
    /// Task<UserData>
    /// </summary>
    public virtual async Task<UserData> FindByUserNameAsync(string userName)
    {
        var userData = await UserLookupServiceProvider.FindByUserNameAsync(userName);
        if (userData == null)
        {
            throw new EntityNotFoundException(typeof(IdentityUser), userName);
        }

        return new UserData(userData);
    }

    /// <summary>
    /// Task<List结果Dto<UserData>>
    /// </summary>
    public virtual async Task<ListResultDto<UserData>> SearchAsync(UserLookupSearchInputDto input)
    {
        var users = await UserLookupServiceProvider.SearchAsync(
            input.Sorting,
            input.Filter,
            input.MaxResultCount,
            input.SkipCount
        );

        return new ListResultDto<UserData>(
            users
                .Select(u => new UserData(u))
                .ToList()
        );
    }

    /// <summary>
    /// Task<long>
    /// </summary>
    public virtual async Task<long> GetCountAsync(UserLookupCountInputDto input)
    {
        return await UserLookupServiceProvider.GetCountAsync(input.Filter);
    }
}
