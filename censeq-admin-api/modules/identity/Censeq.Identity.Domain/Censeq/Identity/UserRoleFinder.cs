using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Censeq.Identity;

/// <summary>
/// 用户角色Finder
/// </summary>
public class UserRoleFinder : IUserRoleFinder, ITransientDependency
{
    /// <summary>
    /// I身份用户仓储
    /// </summary>
    protected IIdentityUserRepository IdentityUserRepository { get; }

    public UserRoleFinder(IIdentityUserRepository identityUserRepository)
    {
        IdentityUserRepository = identityUserRepository;
    }

    [Obsolete("Use GetRoleNamesAsync instead.")]
    /// <summary>
    /// Task<string[]>
    /// </summary>
    public virtual async Task<string[]> GetRolesAsync(Guid userId)
    {
        return (await IdentityUserRepository.GetRoleNamesAsync(userId)).ToArray();
    }

    /// <summary>
    /// Task<string[]>
    /// </summary>
    public async Task<string[]> GetRoleNamesAsync(Guid userId)
    {
        return (await IdentityUserRepository.GetRoleNamesAsync(userId)).ToArray();
    }

    /// <summary>
    /// 异步获取用户拥有的角色 ID 列表。
    /// 包含用户直接拥有的角色，以及通过组织机构继承的角色。
    /// </summary>
    /// <param name="userId">用户标识。</param>
    /// <returns>角色 ID 数组。</returns>
    public async Task<Guid[]> GetRoleIdsAsync(Guid userId)
    {
        return (await IdentityUserRepository.GetRolesAsync(userId)).Select(x => x.Id).ToArray();
    }
}
