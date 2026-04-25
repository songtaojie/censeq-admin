using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Censeq.Identity;

/// <summary>
/// I身份关联用户仓储接口
/// </summary>
public interface IIdentityLinkUserRepository : IBasicRepository<IdentityLinkUser, Guid>
{
    Task<IdentityLinkUser> FindAsync(
        IdentityLinkUserInfo sourceLinkUserInfo,
        IdentityLinkUserInfo targetLinkUserInfo,
        CancellationToken cancellationToken = default);

    Task<List<IdentityLinkUser>> GetListAsync(
        IdentityLinkUserInfo linkUserInfo,
        List<IdentityLinkUserInfo>? excludes = null,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        IdentityLinkUserInfo linkUserInfo,
        CancellationToken cancellationToken = default);
}
