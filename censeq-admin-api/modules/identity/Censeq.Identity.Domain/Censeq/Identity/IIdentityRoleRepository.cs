using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Censeq.Identity;

/// <summary>
/// I身份角色仓储接口
/// </summary>
public interface IIdentityRoleRepository : IBasicRepository<IdentityRole, Guid>
{
    Task<IdentityRole?> FindByIdAsync(
        Guid id,
        bool includeDetails = true,
        CancellationToken cancellationToken = default
    );

    Task<IdentityRole> FindByNormalizedNameAsync(
        string normalizedRoleName,
        bool includeDetails = true,
        CancellationToken cancellationToken = default
    );

    Task<IdentityRole?> FindByCodeAsync(
        string code,
        bool includeDetails = true,
        CancellationToken cancellationToken = default
    );

    Task<List<IdentityRoleWithUserCount>> GetListWithUserCountAsync(
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        string? filter = null,
        bool includeDetails = false,
        CancellationToken cancellationToken = default
    );

    Task<List<IdentityRole>> GetListAsync(
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        string? filter = null,
        bool includeDetails = false,
        CancellationToken cancellationToken = default
    );
    Task<List<IdentityRole>> GetListAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default
    );

    Task<List<IdentityRole>> GetDefaultOnesAsync(
        bool includeDetails = false,
        CancellationToken cancellationToken = default
    );

    Task<long> GetCountAsync(
        string? filter = null,
        CancellationToken cancellationToken = default
    );

    Task RemoveClaimFromAllRolesAsync(
        string claimType,
        bool autoSave = false,
        CancellationToken cancellationToken = default
    );
}
