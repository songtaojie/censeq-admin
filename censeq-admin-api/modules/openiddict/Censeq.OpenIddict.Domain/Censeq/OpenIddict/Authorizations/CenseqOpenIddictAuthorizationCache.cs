using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using OpenIddict.Abstractions;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;

namespace Censeq.OpenIddict.Authorizations;

/// <summary>
/// OpenIddict 授权缓存，封装缓存访问。
/// </summary>
public class CenseqOpenIddictAuthorizationCache : CenseqOpenIddictCacheBase<OpenIddictAuthorization, OpenIddictAuthorizationModel, IOpenIddictAuthorizationStore<OpenIddictAuthorizationModel>>,
    IOpenIddictAuthorizationCache<OpenIddictAuthorizationModel>,
    ITransientDependency
{
    /// <summary>
    /// 初始化 CenseqOpenIddictAuthorizationCache 实例。
    /// </summary>
    /// <param name="cache">缓存。</param>
    /// <param name="arrayCache">array缓存。</param>
    /// <param name="store">存储。</param>
    public CenseqOpenIddictAuthorizationCache(
        IDistributedCache<OpenIddictAuthorizationModel> cache,
        IDistributedCache<OpenIddictAuthorizationModel[]> arrayCache,
        IOpenIddictAuthorizationStore<OpenIddictAuthorizationModel> store)
        : base(cache, arrayCache, store)
    {
    }

    /// <summary>
    /// 异步添加数据。
    /// </summary>
    /// <param name="authorization">OpenIddict 授权。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>表示异步操作的任务。</returns>
    public virtual async ValueTask AddAsync(OpenIddictAuthorizationModel authorization, CancellationToken cancellationToken)
    {
        Check.NotNull(authorization, nameof(authorization));

        await RemoveAsync(authorization, cancellationToken);

        await Cache.SetAsync($"{nameof(FindByIdAsync)}_{await Store.GetIdAsync(authorization, cancellationToken)}", authorization, token: cancellationToken);
    }

    /// <summary>
    /// 异步查找数据。
    /// </summary>
    /// <param name="subject">subject。</param>
    /// <param name="client">客户端标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async IAsyncEnumerable<OpenIddictAuthorizationModel> FindAsync(string subject, string client, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Check.NotNullOrEmpty(subject, nameof(subject));
        Check.NotNullOrEmpty(client, nameof(client));

        var authorizations = await ArrayCache.GetOrAddAsync($"{nameof(FindAsync)}_{subject}_{client}", async () =>
        {
            var applications = new List<OpenIddictAuthorizationModel>();
            await foreach (var authorization in Store.FindAsync(subject, client, cancellationToken))
            {
                applications.Add(authorization);
                await AddAsync(authorization, cancellationToken);
            }
            return applications.ToArray();
        }, token: cancellationToken);

        foreach (var authorization in authorizations)
        {
            yield return authorization;
        }
    }

    /// <summary>
    /// 异步查找数据。
    /// </summary>
    /// <param name="subject">subject。</param>
    /// <param name="client">客户端标识。</param>
    /// <param name="status">status。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async IAsyncEnumerable<OpenIddictAuthorizationModel> FindAsync(string subject, string client, string status, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Check.NotNullOrEmpty(subject, nameof(subject));
        Check.NotNullOrEmpty(client, nameof(client));
        Check.NotNullOrEmpty(status, nameof(status));

        var authorizations = await ArrayCache.GetOrAddAsync($"{nameof(FindAsync)}_{subject}_{client}_{status}", async () =>
        {
            var applications = new List<OpenIddictAuthorizationModel>();
            await foreach (var authorization in Store.FindAsync(subject, client, status, cancellationToken))
            {
                applications.Add(authorization);
                await AddAsync(authorization, cancellationToken);
            }
            return applications.ToArray();
        }, token: cancellationToken);

        foreach (var authorization in authorizations)
        {
            yield return authorization;
        }
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
    public virtual async IAsyncEnumerable<OpenIddictAuthorizationModel> FindAsync(string subject, string client, string status, string type, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Check.NotNullOrEmpty(subject, nameof(subject));
        Check.NotNullOrEmpty(client, nameof(client));
        Check.NotNullOrEmpty(status, nameof(status));
        Check.NotNullOrEmpty(type, nameof(type));

        var authorizations = await ArrayCache.GetOrAddAsync($"{nameof(FindAsync)}_{subject}_{client}_{status}_{type}", async () =>
        {
            var applications = new List<OpenIddictAuthorizationModel>();
            await foreach (var authorization in Store.FindAsync(subject, client, status, type, cancellationToken))
            {
                applications.Add(authorization);
                await AddAsync(authorization, cancellationToken);
            }
            return applications.ToArray();
        }, token: cancellationToken);

        foreach (var authorization in authorizations)
        {
            yield return authorization;
        }
    }

    /// <summary>
    /// 异步查找数据。
    /// </summary>
    /// <param name="subject">subject。</param>
    /// <param name="client">客户端标识。</param>
    /// <param name="status">status。</param>
    /// <param name="type">type。</param>
    /// <param name="scopes">作用域。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async IAsyncEnumerable<OpenIddictAuthorizationModel> FindAsync(string subject, string client, string status, string type, ImmutableArray<string> scopes, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Check.NotNullOrEmpty(subject, nameof(subject));
        Check.NotNullOrEmpty(client, nameof(client));
        Check.NotNullOrEmpty(status, nameof(status));
        Check.NotNullOrEmpty(type, nameof(type));

        // Note: this method is only partially cached.
        await foreach (var authorization in Store.FindAsync(subject, client, status, type, scopes, cancellationToken))
        {
            await AddAsync(authorization, cancellationToken);
            yield return authorization;
        }
    }

    /// <summary>
    /// 根据应用程序标识查找数据。
    /// </summary>
    /// <param name="applicationId">应用程序标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async IAsyncEnumerable<OpenIddictAuthorizationModel> FindByApplicationIdAsync(string applicationId, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Check.NotNullOrEmpty(applicationId, nameof(applicationId));

        var authorizations = await ArrayCache.GetOrAddAsync($"{nameof(FindByApplicationIdAsync)}_{applicationId}", async () =>
        {
            var applications = new List<OpenIddictAuthorizationModel>();
            await foreach (var authorization in Store.FindByApplicationIdAsync(applicationId, cancellationToken))
            {
                applications.Add(authorization);
                await AddAsync(authorization, cancellationToken);
            }
            return applications.ToArray();
        }, token: cancellationToken);

        foreach (var authorization in authorizations)
        {
            yield return authorization;
        }
    }

    /// <summary>
    /// 根据标识查找数据。
    /// </summary>
    /// <param name="id">标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async ValueTask<OpenIddictAuthorizationModel> FindByIdAsync(string id, CancellationToken cancellationToken)
    {
        Check.NotNullOrEmpty(id, nameof(id));

        return await Cache.GetOrAddAsync($"{nameof(FindByIdAsync)}_{id}",
            async () => await Store.FindByIdAsync(id, cancellationToken), token: cancellationToken);
    }

    /// <summary>
    /// 根据主体查找数据。
    /// </summary>
    /// <param name="subject">subject。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async IAsyncEnumerable<OpenIddictAuthorizationModel> FindBySubjectAsync(string subject, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Check.NotNullOrEmpty(subject, nameof(subject));

        var authorizations = await ArrayCache.GetOrAddAsync($"{nameof(FindBySubjectAsync)}_{subject}", async () =>
        {
            var applications = new List<OpenIddictAuthorizationModel>();
            await foreach (var authorization in Store.FindBySubjectAsync(subject, cancellationToken))
            {
                applications.Add(authorization);
                await AddAsync(authorization, cancellationToken);
            }
            return applications.ToArray();
        }, token: cancellationToken);

        foreach (var authorization in authorizations)
        {
            yield return authorization;
        }
    }

    /// <summary>
    /// 异步移除数据。
    /// </summary>
    /// <param name="authorization">OpenIddict 授权。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>表示异步操作的任务。</returns>
    public virtual async ValueTask RemoveAsync(OpenIddictAuthorizationModel authorization, CancellationToken cancellationToken)
    {
        Check.NotNull(authorization, nameof(authorization));

        await ArrayCache.RemoveManyAsync(new[]
        {
            $"{nameof(FindAsync)}_{await Store.GetSubjectAsync(authorization, cancellationToken)}_{await Store.GetApplicationIdAsync(authorization, cancellationToken)}",
            $"{nameof(FindAsync)}_{await Store.GetSubjectAsync(authorization, cancellationToken)}_{await Store.GetApplicationIdAsync(authorization, cancellationToken)}_{await Store.GetStatusAsync(authorization, cancellationToken)}",
            $"{nameof(FindAsync)}_{await Store.GetSubjectAsync(authorization, cancellationToken)}_{await Store.GetApplicationIdAsync(authorization, cancellationToken)}_{await Store.GetStatusAsync(authorization, cancellationToken)}_{await Store.GetTypeAsync(authorization, cancellationToken)}",
            $"{nameof(FindByApplicationIdAsync)}_{await Store.GetApplicationIdAsync(authorization, cancellationToken)}",
            $"{nameof(FindBySubjectAsync)}_{await Store.GetSubjectAsync(authorization, cancellationToken)}"
        }, token: cancellationToken);

        await Cache.RemoveAsync($"{nameof(FindByIdAsync)}_{await Store.GetIdAsync(authorization, cancellationToken)}", token: cancellationToken);
    }
}
