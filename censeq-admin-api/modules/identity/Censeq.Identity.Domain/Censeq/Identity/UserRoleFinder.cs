using System;
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
}
