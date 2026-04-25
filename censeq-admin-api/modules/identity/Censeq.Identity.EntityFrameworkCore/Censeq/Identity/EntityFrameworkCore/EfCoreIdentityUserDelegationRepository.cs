using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Timing;

namespace Censeq.Identity.EntityFrameworkCore;

/// <summary>
/// Ef核心身份用户委托仓储
/// </summary>
public class EfCoreIdentityUserDelegationRepository : EfCoreRepository<ICenseqIdentityDbContext, IdentityUserDelegation, Guid>, IIdentityUserDelegationRepository
{
    /// <summary>
    /// IClock
    /// </summary>
    protected IClock Clock { get; }
    
    public EfCoreIdentityUserDelegationRepository(IDbContextProvider<ICenseqIdentityDbContext> dbContextProvider, IClock clock)
        : base(dbContextProvider)
    {
        Clock = clock;
    }

    /// <summary>
    /// Task<List<Identity用户Delegation>>
    /// </summary>
    public virtual async Task<List<IdentityUserDelegation>> GetListAsync(Guid? sourceUserId, Guid? targetUserId, CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .AsNoTracking()
            .WhereIf(sourceUserId.HasValue, x => x.SourceUserId == sourceUserId)
            .WhereIf(targetUserId.HasValue, x => x.TargetUserId == targetUserId)
            .ToListAsync(cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Task<List<Identity用户Delegation>>
    /// </summary>
    public virtual async Task<List<IdentityUserDelegation>> GetActiveDelegationsAsync(Guid targetUserId, CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .AsNoTracking()
            .Where(x => x.TargetUserId == targetUserId && 
                        x.StartTime <= Clock.Now && 
                        x.EndTime >= Clock.Now)
            .ToListAsync(cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Task<Identity用户Delegation>
    /// </summary>
    public virtual async Task<IdentityUserDelegation> FindActiveDelegationByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return (await (await GetDbSetAsync())
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.StartTime <= Clock.Now &&
                    x.EndTime >= Clock.Now
                , cancellationToken: GetCancellationToken(cancellationToken)))!;
    }
}
