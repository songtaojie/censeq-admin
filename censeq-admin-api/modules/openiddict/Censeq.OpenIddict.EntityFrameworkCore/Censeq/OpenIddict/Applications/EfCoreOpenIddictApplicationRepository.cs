using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Censeq.OpenIddict.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Censeq.OpenIddict.Applications;

/// <summary>
/// EF CoreOpenIddict 应用程序仓储，提供持久化查询能力。
/// </summary>
public class EfCoreOpenIddictApplicationRepository : EfCoreRepository<ICenseqOpenIddictDbContext, OpenIddictApplication, Guid>, IOpenIddictApplicationRepository
{
    /// <summary>
    /// 初始化 EfCoreOpenIddictApplicationRepository 实例。
    /// </summary>
    /// <param name="dbContextProvider">数据库上下文提供者。</param>
    public EfCoreOpenIddictApplicationRepository(IDbContextProvider<ICenseqOpenIddictDbContext> dbContextProvider)
        : base(dbContextProvider)
    {

    }

    /// <summary>
    /// 获取分页数据列表。
    /// </summary>
    /// <param name="sorting">sorting。</param>
    /// <param name="skipCount">skipCount。</param>
    /// <param name="maxResultCount">max结果Count。</param>
    /// <param name="filter">filter。</param>
    /// <param name="clientType">客户端Type。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>分页查询结果。</returns>
    public virtual async Task<List<OpenIddictApplication>> GetListAsync(string sorting, int skipCount, int maxResultCount, string? filter = null,
        string? clientType = null, CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .WhereIf(!filter.IsNullOrWhiteSpace(), x => x.ClientId.Contains(filter!))
            .WhereIf(!clientType.IsNullOrWhiteSpace(), x => x.ClientType == clientType)
            .OrderBy(sorting.IsNullOrWhiteSpace() ? nameof(OpenIddictApplication.ClientId) : sorting)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// 获取数据数量。
    /// </summary>
    /// <param name="filter">filter。</param>
    /// <param name="clientType">客户端Type。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>数据数量。</returns>
    public virtual async Task<long> GetCountAsync(string? filter = null, string? clientType = null, CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .WhereIf(!filter.IsNullOrWhiteSpace(), x => x.ClientId.Contains(filter!))
            .WhereIf(!clientType.IsNullOrWhiteSpace(), x => x.ClientType == clientType)
            .LongCountAsync(GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// 根据客户端标识查找数据。
    /// </summary>
    /// <param name="clientId">客户端标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async Task<OpenIddictApplication> FindByClientIdAsync(string clientId, CancellationToken cancellationToken = default)
    {
        return (await (await GetDbSetAsync())
            .FirstOrDefaultAsync(x => x.ClientId == clientId, GetCancellationToken(cancellationToken)))!;
    }

    /// <summary>
    /// 根据登出后重定向 URI 查找应用程序。
    /// </summary>
    /// <param name="address">地址。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async Task<List<OpenIddictApplication>> FindByPostLogoutRedirectUriAsync(string address, CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .Where(x => x.PostLogoutRedirectUris.Contains(address)).ToListAsync(GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// 根据重定向 URI 查找应用程序。
    /// </summary>
    /// <param name="address">地址。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async Task<List<OpenIddictApplication>> FindByRedirectUriAsync(string address, CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .Where(x => x.RedirectUris.Contains(address)).ToListAsync(GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// 列出数据。
    /// </summary>
    /// <param name="count">count。</param>
    /// <param name="offset">offset。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    public virtual async Task<List<OpenIddictApplication>> ListAsync(int? count, int? offset, CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .OrderBy(x => x.Id)
            .SkipIf<OpenIddictApplication, IQueryable<OpenIddictApplication>>(offset.HasValue, offset)
            .TakeIf<OpenIddictApplication, IQueryable<OpenIddictApplication>>(count.HasValue, count)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }
}
