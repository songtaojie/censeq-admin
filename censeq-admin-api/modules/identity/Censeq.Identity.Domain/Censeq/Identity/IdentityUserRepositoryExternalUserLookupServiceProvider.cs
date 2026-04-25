using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Users;

namespace Censeq.Identity;

/// <summary>
/// 身份用户仓储外部用户查找服务提供程序
/// </summary>
public class IdentityUserRepositoryExternalUserLookupServiceProvider : IExternalUserLookupServiceProvider, ITransientDependency
{
    /// <summary>
    /// I身份用户仓储
    /// </summary>
    protected IIdentityUserRepository UserRepository { get; }
    /// <summary>
    /// I查找Normalizer
    /// </summary>
    protected ILookupNormalizer LookupNormalizer { get; }

    public IdentityUserRepositoryExternalUserLookupServiceProvider(
        IIdentityUserRepository userRepository,
        ILookupNormalizer lookupNormalizer)
    {
        UserRepository = userRepository;
        LookupNormalizer = lookupNormalizer;
    }

    /// <summary>
    /// Task<I用户Data>
    /// </summary>
    public virtual async Task<IUserData> FindByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return (
                await UserRepository.FindAsync(
                    id,
                    includeDetails: false,
                    cancellationToken: cancellationToken
                )
            )?.ToAbpUserData()!;
    }

    /// <summary>
    /// Task<I用户Data>
    /// </summary>
    public virtual async Task<IUserData> FindByUserNameAsync(
        string userName,
        CancellationToken cancellationToken = default)
    {
        return (
                await UserRepository.FindByNormalizedUserNameAsync(
                    LookupNormalizer.NormalizeName(userName),
                    includeDetails: false,
                    cancellationToken: cancellationToken
                )
            )?.ToAbpUserData()!;
    }

    /// <summary>
    /// Task<List<I用户Data>>
    /// </summary>
    public virtual async Task<List<IUserData>> SearchAsync(
        string? sorting = null,
        string? filter = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        var users = await UserRepository.GetListAsync(
            sorting: sorting,
            maxResultCount: maxResultCount,
            skipCount: skipCount,
            filter: filter,
            includeDetails: false,
            cancellationToken: cancellationToken
        );

        return users.Select(u => u.ToAbpUserData()).ToList();
    }

    /// <summary>
    /// Task<long>
    /// </summary>
    public async Task<long> GetCountAsync(
        string? filter = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return await UserRepository.GetCountAsync(filter, cancellationToken: cancellationToken);
    }
}
