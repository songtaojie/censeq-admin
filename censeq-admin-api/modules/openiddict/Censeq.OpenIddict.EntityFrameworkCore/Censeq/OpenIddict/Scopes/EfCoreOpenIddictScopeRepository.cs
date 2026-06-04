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

namespace Censeq.OpenIddict.Scopes;

/// <summary>
/// EF CoreOpenIddict 作用域仓储，提供持久化查询能力。
/// </summary>
public class EfCoreOpenIddictScopeRepository : EfCoreRepository<ICenseqOpenIddictDbContext, OpenIddictScope, Guid>, IOpenIddictScopeRepository
{
    /// <summary>
    /// 初始化 EfCoreOpenIddictScopeRepository 实例。
    /// </summary>
    /// <param name="dbContextProvider">数据库上下文提供者。</param>
    public EfCoreOpenIddictScopeRepository(IDbContextProvider<ICenseqOpenIddictDbContext> dbContextProvider)
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
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>分页查询结果。</returns>
    public virtual async Task<List<OpenIddictScope>> GetListAsync(string sorting, int skipCount, int maxResultCount, string? filter = null,
        CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .WhereIf(!filter.IsNullOrWhiteSpace(), x => 
                x.Name.Contains(filter!) ||
                x.DisplayName.Contains(filter!) ||
                x.Description.Contains(filter!))
            .OrderBy(sorting.IsNullOrWhiteSpace() ? nameof(OpenIddictScope.Name) : sorting)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// 获取数据数量。
    /// </summary>
    /// <param name="filter">filter。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>数据数量。</returns>
    public virtual async Task<long> GetCountAsync(string? filter = null, CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .WhereIf(!filter.IsNullOrWhiteSpace(), x => 
                x.Name.Contains(filter!) ||
                x.DisplayName.Contains(filter!) ||
                x.Description.Contains(filter!))
            .LongCountAsync(GetCancellationToken(cancellationToken));
    }
    
    /// <summary>
    /// 根据标识查找数据。
    /// </summary>
    /// <param name="id">标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async Task<OpenIddictScope> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return (await (await GetQueryableAsync()).FirstOrDefaultAsync(x => x.Id == id, GetCancellationToken(cancellationToken)))!;
    }

    /// <summary>
    /// 根据名称查找数据。
    /// </summary>
    /// <param name="name">name。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async Task<OpenIddictScope> FindByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return (await (await GetQueryableAsync()).FirstOrDefaultAsync(x => x.Name == name, GetCancellationToken(cancellationToken)))!;
    }

    /// <summary>
    /// 根据名称查找数据。
    /// </summary>
    /// <param name="names">名称。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async Task<List<OpenIddictScope>> FindByNamesAsync(string[] names, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).Where(x => names.Contains(x.Name)).ToListAsync(GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// 根据资源查找数据。
    /// </summary>
    /// <param name="resource">资源。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async Task<List<OpenIddictScope>> FindByResourceAsync(string resource, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).Where(x => x.Resources.Contains(resource)).ToListAsync(GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// 列出数据。
    /// </summary>
    /// <param name="count">count。</param>
    /// <param name="offset">offset。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    public virtual async Task<List<OpenIddictScope>> ListAsync(int? count, int? offset, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .OrderBy(x => x.Id)
            .SkipIf<OpenIddictScope, IQueryable<OpenIddictScope>>(offset.HasValue, offset)
            .TakeIf<OpenIddictScope, IQueryable<OpenIddictScope>>(count.HasValue, count)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }
}
