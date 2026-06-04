using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Censeq.OpenIddict.Authorizations;
using Censeq.OpenIddict.EntityFrameworkCore;

namespace Censeq.OpenIddict.Tokens;

/// <summary>
/// EF CoreOpenIddict 令牌仓储，提供持久化查询能力。
/// </summary>
public class EfCoreOpenIddictTokenRepository : EfCoreRepository<ICenseqOpenIddictDbContext, OpenIddictToken, Guid>, IOpenIddictTokenRepository
{
    /// <summary>
    /// 初始化 EfCoreOpenIddictTokenRepository 实例。
    /// </summary>
    /// <param name="dbContextProvider">数据库上下文提供者。</param>
    public EfCoreOpenIddictTokenRepository(IDbContextProvider<ICenseqOpenIddictDbContext> dbContextProvider)
        : base(dbContextProvider)
    {

    }

    /// <summary>
    /// 批量删除数据。
    /// </summary>
    /// <param name="applicationId">应用程序标识。</param>
    /// <param name="autoSave">autoSave。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>表示异步操作的任务。</returns>
    public virtual async Task DeleteManyByApplicationIdAsync(Guid applicationId, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        var tokens = await (await GetDbSetAsync())
            .Where(x => x.ApplicationId == applicationId)
            .ToListAsync(GetCancellationToken(cancellationToken));

        await DeleteManyAsync(tokens, autoSave, cancellationToken);
    }

    /// <summary>
    /// 批量删除数据。
    /// </summary>
    /// <param name="authorizationId">授权标识。</param>
    /// <param name="autoSave">autoSave。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>表示异步操作的任务。</returns>
    public virtual async Task DeleteManyByAuthorizationIdAsync(Guid authorizationId, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        var tokens = await (await GetDbSetAsync())
            .Where(x => x.AuthorizationId == authorizationId)
            .ToListAsync(GetCancellationToken(cancellationToken));

        await DeleteManyAsync(tokens, autoSave, GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// 批量删除数据。
    /// </summary>
    /// <param name="authorizationIds">授权标识s。</param>
    /// <param name="autoSave">autoSave。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>表示异步操作的任务。</returns>
    public virtual async Task DeleteManyByAuthorizationIdsAsync(Guid[] authorizationIds, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        var tokens = await (await GetDbSetAsync())
            .Where(x => x.AuthorizationId != null && authorizationIds.Contains(x.AuthorizationId.Value))
            .ToListAsync(GetCancellationToken(cancellationToken));

        await DeleteManyAsync(tokens, autoSave, GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// 异步查找数据。
    /// </summary>
    /// <param name="subject">subject。</param>
    /// <param name="client">客户端标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async Task<List<OpenIddictToken>> FindAsync(string subject, Guid client, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).Where(x => x.Subject == subject && x.ApplicationId == client).ToListAsync(GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// 异步查找数据。
    /// </summary>
    /// <param name="subject">subject。</param>
    /// <param name="client">客户端标识。</param>
    /// <param name="status">status。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async Task<List<OpenIddictToken>> FindAsync(string subject, Guid client, string status, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).Where(x => x.Subject == subject && x.ApplicationId == client && x.Status == status).ToListAsync(GetCancellationToken(cancellationToken));
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
    public virtual async Task<List<OpenIddictToken>> FindAsync(string subject, Guid client, string status, string type, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).Where(x => x.Subject == subject && x.ApplicationId == client && x.Status == status && x.Type == type).ToListAsync(GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// 根据应用程序标识查找数据。
    /// </summary>
    /// <param name="applicationId">应用程序标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async Task<List<OpenIddictToken>> FindByApplicationIdAsync(Guid applicationId, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).Where(x => x.ApplicationId == applicationId).ToListAsync(GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// 根据授权标识查找数据。
    /// </summary>
    /// <param name="authorizationId">授权标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async Task<List<OpenIddictToken>> FindByAuthorizationIdAsync(Guid authorizationId, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).Where(x => x.AuthorizationId == authorizationId).ToListAsync(GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// 根据标识查找数据。
    /// </summary>
    /// <param name="id">标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async Task<OpenIddictToken> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return (await (await GetQueryableAsync()).FirstOrDefaultAsync(x => x.Id == id, GetCancellationToken(cancellationToken)))!;
    }

    /// <summary>
    /// 根据引用标识查找数据。
    /// </summary>
    /// <param name="referenceId">reference标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async Task<OpenIddictToken> FindByReferenceIdAsync(string referenceId, CancellationToken cancellationToken = default)
    {
        return (await (await GetQueryableAsync()).FirstOrDefaultAsync(x => x.ReferenceId == referenceId, GetCancellationToken(cancellationToken)))!;
    }

    /// <summary>
    /// 根据主体查找数据。
    /// </summary>
    /// <param name="subject">subject。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async Task<List<OpenIddictToken>> FindBySubjectAsync(string subject, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).Where(x => x.Subject == subject).ToListAsync(GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// 列出数据。
    /// </summary>
    /// <param name="count">count。</param>
    /// <param name="offset">offset。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    public virtual async Task<List<OpenIddictToken>> ListAsync(int? count, int? offset, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .OrderBy(x => x.Id)
            .SkipIf<OpenIddictToken, IQueryable<OpenIddictToken>>(offset.HasValue, offset)
            .TakeIf<OpenIddictToken, IQueryable<OpenIddictToken>>(count.HasValue, count)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// 清理过期或无效数据。
    /// </summary>
    /// <param name="date">date。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    public virtual async Task<long> PruneAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        return await (from token in await GetQueryableAsync()
                join authorization in (await GetDbContextAsync()).Set<OpenIddictAuthorization>()
                    on token.AuthorizationId equals authorization.Id into tokenAuthorizations
                from tokenAuthorization in tokenAuthorizations.DefaultIfEmpty()
                where token.CreationDate < date
                where (token.Status != OpenIddictConstants.Statuses.Inactive && token.Status != OpenIddictConstants.Statuses.Valid) ||
                      (tokenAuthorization != null && tokenAuthorization.Status != OpenIddictConstants.Statuses.Valid) ||
                      token.ExpirationDate < DateTime.UtcNow
                select token)
            .ExecuteDeleteAsync(GetCancellationToken(cancellationToken));
    }

    /// <summary>
    /// 撤销数据。
    /// </summary>
    /// <param name="id">标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    public virtual async ValueTask<long> RevokeByAuthorizationIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await (from token in await GetQueryableAsync() where token.AuthorizationId == id select token)
            .ExecuteUpdateAsync(
                entity => entity.SetProperty(token => token.Status, OpenIddictConstants.Statuses.Revoked),
                GetCancellationToken(cancellationToken));
    }
}
