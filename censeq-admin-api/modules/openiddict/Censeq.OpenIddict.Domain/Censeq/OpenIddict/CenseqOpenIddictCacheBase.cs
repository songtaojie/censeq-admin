using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Volo.Abp.Caching;

namespace Censeq.OpenIddict;

public class CenseqOpenIddictCacheBase<TEntity, TModel, TStore>
    where TModel : class
    where TEntity : class
{
    public ILogger<CenseqOpenIddictCacheBase<TEntity, TModel, TStore>> Logger { get; set; }

    protected IDistributedCache<TModel> Cache { get; }

    protected IDistributedCache<TModel[]> ArrayCache { get; }

    protected TStore Store { get; }

    protected CenseqOpenIddictCacheBase(IDistributedCache<TModel> cache, IDistributedCache<TModel[]> arrayCache, TStore store)
    {
        Cache = cache;
        ArrayCache = arrayCache;
        Store = store;

        Logger = NullLogger<CenseqOpenIddictCacheBase<TEntity, TModel, TStore>>.Instance;
    }
}
