using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Censeq.OpenIddict.EntityFrameworkCore;
using Censeq.OpenIddict.Tokens;

namespace Censeq.OpenIddict.Authorizations;

/// <summary>
/// EF CoreOpenIddict 授权仓储，提供持久化查询能力。
/// </summary>
public class EfCoreOpenIddictAuthorizationRepository : EfCoreRepository<ICenseqOpenIddictDbContext, OpenIddictAuthorization, Guid>, IOpenIddictAuthorizationRepository
{
    /// <summary>
    /// 初始化 EfCoreOpenIddictAuthorizationRepository 实例。
    /// </summary>
    /// <param name="dbContextProvider">数据库上下文提供者。</param>
    public EfCoreOpenIddictAuthorizationRepository(IDbContextProvider<ICenseqOpenIddictDbContext> dbContextProvider)
        : base(dbContextProvider)
    {

    }

    /// <summary>
    /// 异步查找数据。
    /// </summary>
    /// <param name="subject">subject。</param>
    /// <param name="client">客户端标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async Task<List<OpenIddictAuthorization>> FindAsync(string subject, Guid client, CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .Where(x => x.Subject == subject && x.ApplicationId == client)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// 异步查找数据。
    /// </summary>
    /// <param name="subject">subject。</param>
    /// <param name="client">客户端标识。</param>
    /// <param name="status">status。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async Task<List<OpenIddictAuthorization>> FindAsync(string subject, Guid client, string status, CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .Where(x => x.Subject == subject && x.Status == status && x.ApplicationId == client)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// 异步查找数据。
    /// </summary>
    /// <param name="subject">subject。</param>
    /// <param name="client">客户端标识。</param>
    /// <param name="status">status。</param>
    /// <param name="type">type。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async Task<List<OpenIddictAuthorization>> FindAsync(string subject, Guid client, string status, string type, CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .Where(x => x.Subject == subject && x.Status == status && x.Type == type && x.ApplicationId == client)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// 根据应用程序标识查找数据。
    /// </summary>
    /// <param name="applicationId">应用程序标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async Task<List<OpenIddictAuthorization>> FindByApplicationIdAsync(Guid applicationId, CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .Where(x => x.ApplicationId == applicationId)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// 根据标识查找数据。
    /// </summary>
    /// <param name="id">标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async Task<OpenIddictAuthorization> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return (await (await GetDbSetAsync())
            .FirstOrDefaultAsync(x => x.Id == id, GetCancellationToken(cancellationToken)))!;
    }

    /// <summary>
    /// 根据主体查找数据。
    /// </summary>
    /// <param name="subject">subject。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async Task<List<OpenIddictAuthorization>> FindBySubjectAsync(string subject, CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .Where(x => x.Subject == subject)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// 列出数据。
    /// </summary>
    /// <param name="count">count。</param>
    /// <param name="offset">offset。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    public virtual async Task<List<OpenIddictAuthorization>> ListAsync(int? count, int? offset, CancellationToken cancellationToken = default)
    {
        var query = (await GetDbSetAsync())
            .OrderBy(authorization => authorization.Id!)
            .AsTracking();

        if (offset.HasValue)
        {
            query = query.Skip(offset.Value);
        }

        if (count.HasValue)
        {
            query = query.Take(count.Value);
        }

        return await query.ToListAsync(GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// 清理过期或无效数据。
    /// </summary>
    /// <param name="date">date。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    public virtual async Task<long> PruneAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        var authorizations = await (from authorization in (await GetQueryableAsync())
            join token in (await GetDbContextAsync()).Set<OpenIddictToken>()
                on authorization.Id equals token.AuthorizationId into authorizationTokens
            from authorizationToken in authorizationTokens.DefaultIfEmpty()
            where authorization.CreationDate < date
            where authorization.Status != OpenIddictConstants.Statuses.Valid ||
                  (authorization.Type == OpenIddictConstants.AuthorizationTypes.AdHoc && authorizationToken == null)
            select authorization.Id).ToListAsync(cancellationToken);

        var count = await (from token in (await GetDbContextAsync()).Set<OpenIddictToken>()
                where token.AuthorizationId != null && authorizations.Contains(token.AuthorizationId.Value)
                select token)
            .ExecuteDeleteAsync(GetCancellationToken(cancellationToken));

        return count + await (await GetDbSetAsync()).Where(x => authorizations.Contains(x.Id)).ExecuteDeleteAsync(cancellationToken);
    }
}
