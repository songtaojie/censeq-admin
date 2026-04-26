using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Censeq.Identity;

/// <summary>
/// I身份声明类型仓储接口
/// </summary>
public interface IIdentityClaimTypeRepository : IBasicRepository<IdentityClaimType, Guid>
{
    /// <summary>
    /// 检查是否存在具有指定名称的 <see cref="IdentityClaimType"/> 实体。
    /// </summary>
    /// <param name="name">要检查的名称</param>
    /// <param name="ignoredId">
    /// 检查时要忽略的标识值。
    /// 如果存在具有给定 <paramref name="ignoredId"/> 的实体，则忽略它。
    /// </param>
    /// <param name="cancellationToken">取消令牌</param>
    Task<bool> AnyAsync(
        string name,
        Guid? ignoredId = null,
        CancellationToken cancellationToken = default
    );

    Task<List<IdentityClaimType>> GetListAsync(
        string? sorting,
        int maxResultCount,
        int skipCount,
        string filter,
        CancellationToken cancellationToken = default
    );

    Task<long> GetCountAsync(
        string? filter = null,
        CancellationToken cancellationToken = default
    );
    
    Task<List<IdentityClaimType>> GetListByNamesAsync(
        IEnumerable<string> names,
        CancellationToken cancellationToken = default
    );
}
