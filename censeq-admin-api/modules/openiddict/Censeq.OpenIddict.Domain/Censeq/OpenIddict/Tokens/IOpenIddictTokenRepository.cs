using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Censeq.OpenIddict.Tokens;

/// <summary>
/// OpenIddict 令牌仓储接口。
/// </summary>
public interface IOpenIddictTokenRepository : IBasicRepository<OpenIddictToken, Guid>
{
    /// <summary>
    /// 批量删除数据。
    /// </summary>
    /// <param name="applicationId">应用程序标识。</param>
    /// <param name="autoSave">autoSave。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>表示异步操作的任务。</returns>
    Task DeleteManyByApplicationIdAsync(Guid applicationId, bool autoSave = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量删除数据。
    /// </summary>
    /// <param name="authorizationId">授权标识。</param>
    /// <param name="autoSave">autoSave。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>表示异步操作的任务。</returns>
    Task DeleteManyByAuthorizationIdAsync(Guid authorizationId, bool autoSave = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量删除数据。
    /// </summary>
    /// <param name="authorizationIds">授权标识s。</param>
    /// <param name="autoSave">autoSave。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>表示异步操作的任务。</returns>
    Task DeleteManyByAuthorizationIdsAsync(Guid[] authorizationIds, bool autoSave = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步查找数据。
    /// </summary>
    /// <param name="subject">subject。</param>
    /// <param name="client">客户端标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    Task<List<OpenIddictToken>> FindAsync(string subject, Guid client, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步查找数据。
    /// </summary>
    /// <param name="subject">subject。</param>
    /// <param name="client">客户端标识。</param>
    /// <param name="status">status。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    Task<List<OpenIddictToken>> FindAsync(string subject, Guid client, string status, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步查找数据。
    /// </summary>
    /// <param name="subject">subject。</param>
    /// <param name="client">客户端标识。</param>
    /// <param name="status">status。</param>
    /// <param name="type">type。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    Task<List<OpenIddictToken>> FindAsync(string subject, Guid client, string status, string type, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据应用程序标识查找数据。
    /// </summary>
    /// <param name="applicationId">应用程序标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    Task<List<OpenIddictToken>> FindByApplicationIdAsync(Guid applicationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据授权标识查找数据。
    /// </summary>
    /// <param name="authorizationId">授权标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    Task<List<OpenIddictToken>> FindByAuthorizationIdAsync(Guid authorizationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据标识查找数据。
    /// </summary>
    /// <param name="id">标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    Task<OpenIddictToken> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据引用标识查找数据。
    /// </summary>
    /// <param name="referenceId">reference标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    Task<OpenIddictToken> FindByReferenceIdAsync(string referenceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据主体查找数据。
    /// </summary>
    /// <param name="subject">subject。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    Task<List<OpenIddictToken>> FindBySubjectAsync(string subject, CancellationToken cancellationToken = default);

    /// <summary>
    /// 列出数据。
    /// </summary>
    /// <param name="count">count。</param>
    /// <param name="offset">offset。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    Task<List<OpenIddictToken>> ListAsync(int? count, int? offset, CancellationToken cancellationToken = default);

    /// <summary>
    /// 清理过期或无效数据。
    /// </summary>
    /// <param name="date">date。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    Task<long> PruneAsync(DateTime date, CancellationToken cancellationToken = default);

    /// <summary>
    /// 撤销数据。
    /// </summary>
    /// <param name="id">标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    ValueTask<long> RevokeByAuthorizationIdAsync(Guid id, CancellationToken cancellationToken);
}
