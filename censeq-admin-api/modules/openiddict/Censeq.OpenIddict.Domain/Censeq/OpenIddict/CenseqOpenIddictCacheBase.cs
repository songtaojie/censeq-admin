using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Volo.Abp.Caching;

namespace Censeq.OpenIddict;

/// <summary>
/// OpenIddict缓存基类。
/// </summary>
public class CenseqOpenIddictCacheBase<TEntity, TModel, TStore>
    where TModel : class
    where TEntity : class
{
    /// <summary>
    /// 日志记录器。
    /// </summary>
    public ILogger<CenseqOpenIddictCacheBase<TEntity, TModel, TStore>> Logger { get; set; }

    /// <summary>
    /// 缓存。
    /// </summary>
    protected IDistributedCache<TModel> Cache { get; }

    /// <summary>
    /// 数组缓存。
    /// </summary>
    protected IDistributedCache<TModel[]> ArrayCache { get; }

    /// <summary>
    /// 存储。
    /// </summary>
    protected TStore Store { get; }

    /// <summary>
    /// 初始化 CenseqOpenIddictCacheBase 实例。
    /// </summary>
    /// <param name="cache">缓存。</param>
    /// <param name="arrayCache">array缓存。</param>
    /// <param name="store">存储。</param>
    protected CenseqOpenIddictCacheBase(IDistributedCache<TModel> cache, IDistributedCache<TModel[]> arrayCache, TStore store)
    {
        Cache = cache;
        ArrayCache = arrayCache;
        Store = store;

        Logger = NullLogger<CenseqOpenIddictCacheBase<TEntity, TModel, TStore>>.Instance;
    }
}
