using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using OpenIddict.Abstractions;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;

namespace Censeq.OpenIddict.Scopes;

/// <summary>
/// OpenIddict 作用域缓存，封装缓存访问。
/// </summary>
public class CenseqOpenIddictScopeCache : CenseqOpenIddictCacheBase<OpenIddictScope, OpenIddictScopeModel, IOpenIddictScopeStore<OpenIddictScopeModel>>,
    IOpenIddictScopeCache<OpenIddictScopeModel>,
    ITransientDependency
{
    /// <summary>
    /// 初始化 CenseqOpenIddictScopeCache 实例。
    /// </summary>
    /// <param name="cache">缓存。</param>
    /// <param name="arrayCache">array缓存。</param>
    /// <param name="store">存储。</param>
    public CenseqOpenIddictScopeCache(
        IDistributedCache<OpenIddictScopeModel> cache,
        IDistributedCache<OpenIddictScopeModel[]> arrayCache,
        IOpenIddictScopeStore<OpenIddictScopeModel> store)
        : base(cache, arrayCache, store)
    {
    }

    /// <summary>
    /// 异步添加数据。
    /// </summary>
    /// <param name="scope">OpenIddict 作用域。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>表示异步操作的任务。</returns>
    public virtual async ValueTask AddAsync(OpenIddictScopeModel scope, CancellationToken cancellationToken)
    {
        Check.NotNull(scope, nameof(scope));

        await RemoveAsync(scope, cancellationToken);

        await Cache.SetAsync($"{nameof(FindByIdAsync)}_{await Store.GetIdAsync(scope, cancellationToken)}", scope, token: cancellationToken);
        await Cache.SetAsync($"{nameof(FindByNameAsync)}_{await Store.GetNameAsync(scope, cancellationToken)}", scope, token: cancellationToken);
    }

    /// <summary>
    /// 根据标识查找数据。
    /// </summary>
    /// <param name="id">标识。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async ValueTask<OpenIddictScopeModel> FindByIdAsync(string id, CancellationToken cancellationToken)
    {
        Check.NotNullOrEmpty(id, nameof(id));

        return await Cache.GetOrAddAsync($"{nameof(FindByIdAsync)}_{id}",  async () =>
        {
            var scope = await Store.FindByIdAsync(id, cancellationToken);
            if (scope != null)
            {
                await AddAsync(scope, cancellationToken);
            }
            return scope;
        }, token: cancellationToken);
    }

    /// <summary>
    /// 根据名称查找数据。
    /// </summary>
    /// <param name="name">name。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async ValueTask<OpenIddictScopeModel> FindByNameAsync(string name, CancellationToken cancellationToken)
    {
        Check.NotNullOrEmpty(name, nameof(name));

        return await Cache.GetOrAddAsync($"{nameof(FindByNameAsync)}_{name}",  async () =>
        {
            var scope = await Store.FindByNameAsync(name, cancellationToken);
            if (scope != null)
            {
                await AddAsync(scope, cancellationToken);
            }
            return scope;
        }, token: cancellationToken);
    }

    /// <summary>
    /// 根据名称查找数据。
    /// </summary>
    /// <param name="names">名称。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async IAsyncEnumerable<OpenIddictScopeModel> FindByNamesAsync(ImmutableArray<string> names, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Check.NotNull(names, nameof(names));

        foreach (var name in names)
        {
            Check.NotNullOrEmpty(name, nameof(name));
        }

        // Note: this method is only partially cached.
        await foreach (var scope in Store.FindByNamesAsync(names, cancellationToken))
        {
            await AddAsync(scope, cancellationToken);
            yield return scope;
        }
    }

    /// <summary>
    /// 根据资源查找数据。
    /// </summary>
    /// <param name="resource">资源。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>匹配的数据。</returns>
    public virtual async IAsyncEnumerable<OpenIddictScopeModel> FindByResourceAsync(string resource, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Check.NotNullOrEmpty(resource, nameof(resource));

        var scopes = await ArrayCache.GetOrAddAsync($"{nameof(FindByResourceAsync)}_{resource}", async () =>
        {
            var scopes = new List<OpenIddictScopeModel>();
            await foreach (var scope in Store.FindByResourceAsync(resource, cancellationToken))
            {
                scopes.Add(scope);
                await AddAsync(scope, cancellationToken);
            }
            return scopes.ToArray();
        }, token: cancellationToken);

        foreach (var scope in scopes)
        {
            yield return scope;
        }
    }

    /// <summary>
    /// 异步移除数据。
    /// </summary>
    /// <param name="scope">OpenIddict 作用域。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>表示异步操作的任务。</returns>
    public virtual async ValueTask RemoveAsync(OpenIddictScopeModel scope, CancellationToken cancellationToken)
    {
        Check.NotNull(scope, nameof(scope));

        var resources = new List<string>();
        foreach (var resource in await Store.GetResourcesAsync(scope, cancellationToken))
        {
            resources.Add($"{nameof(FindByResourceAsync)}_{resource}");
        }
        await ArrayCache.RemoveManyAsync(resources.ToArray(), token: cancellationToken);

        await Cache.RemoveAsync($"{nameof(FindByIdAsync)}_{await Store.GetIdAsync(scope, cancellationToken)}", token: cancellationToken);
        await Cache.RemoveAsync($"{nameof(FindByNameAsync)}_{await Store.GetNameAsync(scope, cancellationToken)}", token: cancellationToken);
    }
}
