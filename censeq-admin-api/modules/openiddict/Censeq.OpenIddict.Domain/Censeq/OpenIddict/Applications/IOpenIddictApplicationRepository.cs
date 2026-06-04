using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Censeq.OpenIddict.Applications;

/// <summary>
/// OpenIddict 应用程序仓储接口。
/// </summary>
public interface IOpenIddictApplicationRepository : IRepository<OpenIddictApplication, Guid>
{
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
    Task<List<OpenIddictApplication>> GetListAsync(string sorting, int skipCount, int maxResultCount, string filter = null, string clientType = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取数据数量。
    /// </summary>
    /// <param name="filter">filter。</param>
    /// <param name="clientType">客户端Type。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>数据数量。</returns>
    Task<long> GetCountAsync(string filter = null, string clientType = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 根据客户端标识查找数据。
    /// </summary>
    /// <param name="clientId">客户端标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    Task<OpenIddictApplication> FindByClientIdAsync(string clientId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据登出后重定向 URI 查找应用程序。
    /// </summary>
    /// <param name="address">地址。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    Task<List<OpenIddictApplication>> FindByPostLogoutRedirectUriAsync(string address, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据重定向 URI 查找应用程序。
    /// </summary>
    /// <param name="address">地址。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    Task<List<OpenIddictApplication>> FindByRedirectUriAsync(string address, CancellationToken cancellationToken = default);

    /// <summary>
    /// 列出数据。
    /// </summary>
    /// <param name="count">count。</param>
    /// <param name="offset">offset。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    Task<List<OpenIddictApplication>> ListAsync(int? count, int? offset, CancellationToken cancellationToken = default);
}
