using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Censeq.OpenIddict.Scopes;

/// <summary>
/// OpenIddict 作用域仓储接口。
/// </summary>
public interface IOpenIddictScopeRepository : IRepository<OpenIddictScope, Guid>
{
    /// <summary>
    /// 获取分页数据列表。
    /// </summary>
    /// <param name="sorting">sorting。</param>
    /// <param name="skipCount">skipCount。</param>
    /// <param name="maxResultCount">max结果Count。</param>
    /// <param name="filter">filter。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>分页查询结果。</returns>
    Task<List<OpenIddictScope>> GetListAsync(string sorting, int skipCount, int maxResultCount, string filter = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取数据数量。
    /// </summary>
    /// <param name="filter">filter。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>数据数量。</returns>
    Task<long> GetCountAsync(string filter = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 根据标识查找数据。
    /// </summary>
    /// <param name="id">标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    Task<OpenIddictScope> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据名称查找数据。
    /// </summary>
    /// <param name="name">name。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    Task<OpenIddictScope> FindByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据名称查找数据。
    /// </summary>
    /// <param name="names">名称。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    Task<List<OpenIddictScope>> FindByNamesAsync(string[] names, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据资源查找数据。
    /// </summary>
    /// <param name="resource">资源。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    Task<List<OpenIddictScope>> FindByResourceAsync(string resource, CancellationToken cancellationToken = default);

    /// <summary>
    /// 列出数据。
    /// </summary>
    /// <param name="count">count。</param>
    /// <param name="offset">offset。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    Task<List<OpenIddictScope>> ListAsync(int? count, int? offset, CancellationToken cancellationToken = default);
}
