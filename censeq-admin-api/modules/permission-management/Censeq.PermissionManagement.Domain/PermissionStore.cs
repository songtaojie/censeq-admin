using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Volo.Abp;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;

namespace Censeq.PermissionManagement;

/// <summary>
/// 权限存储。
/// 为 ABP 权限检查流程提供授权读取能力，并通过分布式缓存减少仓储查询。
/// </summary>
public class PermissionStore : IPermissionStore, ITransientDependency
{
    /// <summary>
    /// 日志记录
    /// </summary>
    protected ILogger<PermissionStore> Logger { get; set; }

    /// <summary>
    /// 权限授予存储库
    /// </summary>
    protected IPermissionGrantRepository PermissionGrantRepository { get; }

    /// <summary>
    /// 权限定义管理器
    /// </summary>
    protected IPermissionDefinitionManager PermissionDefinitionManager { get; }

    /// <summary>
    /// 分布式缓存
    /// </summary>
    protected IDistributedCache<PermissionGrantCacheItem> Cache { get; }

    /// <summary>
    /// 初始化权限存储。
    /// </summary>
    /// <param name="permissionGrantRepository">权限授予存储库</param>
    /// <param name="cache">分布式缓存</param>
    /// <param name="permissionDefinitionManager">权限定义管理器</param>
    public PermissionStore(IPermissionGrantRepository permissionGrantRepository, IDistributedCache<PermissionGrantCacheItem> cache, IPermissionDefinitionManager permissionDefinitionManager)
    {
        PermissionGrantRepository = permissionGrantRepository;
        Cache = cache;
        PermissionDefinitionManager = permissionDefinitionManager;
        Logger = NullLogger<PermissionStore>.Instance;
    }

    /// <summary>
    /// 判断指定权限是否已授予给指定提供者。
    /// </summary>
    /// <param name="name">权限名称。</param>
    /// <param name="providerName">权限提供者名称。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    /// <returns>已授予返回 true，否则返回 false。</returns>
    public virtual async Task<bool> IsGrantedAsync(string name, string providerName, string providerKey)
    {
        return (await GetCacheItemAsync(name, providerName, providerKey)).IsGranted;
    }

    /// <summary>
    /// 获取权限缓存项。
    /// 缓存未命中时会加载同一提供者下的权限授予数据并回填缓存。
    /// </summary>
    /// <param name="name">权限名称。</param>
    /// <param name="providerName">权限提供者名称。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    /// <returns>权限授予缓存项。</returns>
    protected virtual async Task<PermissionGrantCacheItem> GetCacheItemAsync(string name, string providerName, string providerKey)
    {
        var cacheKey = CalculateCacheKey(name, providerName, providerKey);
        Logger.LogDebug($"权限存储.GetCacheItemAsync: {cacheKey}");
        var cacheItem = await Cache.GetAsync(cacheKey);

        if (cacheItem != null)
        {
            Logger.LogDebug($"在缓存中找到: {cacheKey}");
            return cacheItem;
        }
        Logger.LogDebug($"缓存中未找到: {cacheKey}");
        cacheItem = new PermissionGrantCacheItem(false);
        await SetCacheItemsAsync(providerName, providerKey, name, cacheItem);
        return cacheItem;
    }

    /// <summary>
    /// 设置权限缓存项。
    /// 单权限查询未命中时，会顺带缓存该提供者下全部已定义权限的授予状态。
    /// </summary>
    /// <param name="providerName">权限提供者名称。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    /// <param name="currentName">当前查询的权限名称。</param>
    /// <param name="currentCacheItem">当前查询要返回的缓存项。</param>
    /// <returns>异步任务。</returns>
    protected virtual async Task SetCacheItemsAsync(string providerName, string providerKey, string currentName, PermissionGrantCacheItem currentCacheItem)
    {
        var permissions = await PermissionDefinitionManager.GetPermissionsAsync();

        Logger.LogDebug($"从存储库中获取此提供商名称的所有已授予的权限,key: {providerName},{providerKey}");

        var grantedPermissionsHashSet = new HashSet<string>(
            (await PermissionGrantRepository.GetListAsync(providerName, providerKey)).Select(p => p.Name)
        );

        Logger.LogDebug($"Setting the cache items. Count: {permissions.Count}");

        var cacheItems = new List<KeyValuePair<string, PermissionGrantCacheItem>>();

        foreach (var permission in permissions)
        {
            var isGranted = grantedPermissionsHashSet.Contains(permission.Name);

            cacheItems.Add(new KeyValuePair<string, PermissionGrantCacheItem>(
                CalculateCacheKey(permission.Name, providerName, providerKey),
                new PermissionGrantCacheItem(isGranted))
            );

            if (permission.Name == currentName)
            {
                currentCacheItem.IsGranted = isGranted;
            }
        }

        await Cache.SetManyAsync(cacheItems);

        Logger.LogDebug($"完成缓存项的设置. 数量: {permissions.Count}");
    }

    /// <summary>
    /// 批量判断权限是否已授予给指定提供者。
    /// </summary>
    /// <param name="names">权限名称集合。</param>
    /// <param name="providerName">权限提供者名称。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    /// <returns>批量权限检查结果。</returns>
    public virtual async Task<MultiplePermissionGrantResult> IsGrantedAsync(string[] names, string providerName, string providerKey)
    {
        Check.NotNullOrEmpty(names, nameof(names));

        var result = new MultiplePermissionGrantResult();

        if (names.Length == 1)
        {
            var name = names.First();
            result.Result.Add(name,
                await IsGrantedAsync(names.First(), providerName, providerKey)
                    ? PermissionGrantResult.Granted
                    : PermissionGrantResult.Undefined);
            return result;
        }

        var cacheItems = await GetCacheItemsAsync(names, providerName, providerKey);
        foreach (var item in cacheItems)
        {
            result.Result.Add(GetPermissionNameFormCacheKeyOrNull(item.Key)!,
                item.Value != null && item.Value.IsGranted
                    ? PermissionGrantResult.Granted
                    : PermissionGrantResult.Undefined);
        }

        return result;
    }

    /// <summary>
    /// 批量获取权限缓存项。
    /// 仅对未命中的缓存键重新查询仓储并回填，避免重复读取已命中的缓存项。
    /// </summary>
    /// <param name="names">权限名称集合。</param>
    /// <param name="providerName">权限提供者名称。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    /// <returns>权限缓存键和值的列表。</returns>
    protected virtual async Task<List<KeyValuePair<string, PermissionGrantCacheItem>>> GetCacheItemsAsync(string[] names, string providerName, string providerKey)
    {
        var cacheKeys = names.Select(x => CalculateCacheKey(x, providerName, providerKey)).ToList();

        Logger.LogDebug($"权限存储.GetCacheItemAsync: {string.Join(",", cacheKeys)}");

        var cacheItems = (await Cache.GetManyAsync(cacheKeys)).ToList();
        if (cacheItems.All(x => x.Value != null))
        {
            Logger.LogDebug($"在缓存中找到: {string.Join(",", cacheKeys)}");
            return cacheItems!;
        }

        var notCacheKeys = cacheItems.Where(x => x.Value == null).Select(x => x.Key).ToList();

        Logger.LogDebug($"缓存中未找到: {string.Join(",", notCacheKeys)}");

        var newCacheItems = await SetCacheItemsAsync(providerName, providerKey, notCacheKeys);

        var result = new List<KeyValuePair<string, PermissionGrantCacheItem>>();
        foreach (var key in cacheKeys)
        {
            var item = newCacheItems.FirstOrDefault(x => x.Key == key);
            if (item.Value == null)
            {
                item = cacheItems.FirstOrDefault(x => x.Key == key)!;
            }

            result.Add(new KeyValuePair<string, PermissionGrantCacheItem>(key, item.Value));
        }

        return result;
    }

    /// <summary>
    /// 为批量查询中未命中的权限缓存键重新设置缓存项。
    /// </summary>
    /// <param name="providerName">权限提供者名称。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    /// <param name="notCacheKeys">未命中的权限缓存键。</param>
    /// <returns>新写入的缓存项。</returns>
    protected virtual async Task<List<KeyValuePair<string, PermissionGrantCacheItem>>> SetCacheItemsAsync(
        string providerName,
        string providerKey,
        List<string> notCacheKeys)
    {
        var permissions = (await PermissionDefinitionManager.GetPermissionsAsync())
            .Where(x => notCacheKeys.Any(k => GetPermissionNameFormCacheKeyOrNull(k) == x.Name)).ToList();
        Logger.LogDebug($"从存储库中获取此提供商名称未缓存授予的权限,key: {providerName},{providerKey}");
        var grantedPermissionsHashSet = new HashSet<string>(
            (await PermissionGrantRepository.GetListAsync(notCacheKeys.Select(GetPermissionNameFormCacheKeyOrNull).ToArray()!, providerName, providerKey)).Select(p => p.Name)
        );
        Logger.LogDebug($"设置缓存项. 数量: {permissions.Count}");

        var cacheItems = new List<KeyValuePair<string, PermissionGrantCacheItem>>();

        foreach (var permission in permissions)
        {
            var isGranted = grantedPermissionsHashSet.Contains(permission.Name);

            cacheItems.Add(new KeyValuePair<string, PermissionGrantCacheItem>(
                CalculateCacheKey(permission.Name, providerName, providerKey),
                new PermissionGrantCacheItem(isGranted))
            );
        }

        await Cache.SetManyAsync(cacheItems);
        Logger.LogDebug($"完成缓存项的设置. 数量: {permissions.Count}");
        return cacheItems;
    }

    /// <summary>
    /// 生成权限授予缓存键。
    /// </summary>
    /// <param name="name">权限名称。</param>
    /// <param name="providerName">权限提供者名称。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    /// <returns>权限授予缓存键。</returns>
    protected virtual string CalculateCacheKey(string name, string providerName, string providerKey)
    {
        return PermissionGrantCacheItem.CalculateCacheKey(name, providerName, providerKey);
    }

    /// <summary>
    /// 从权限授予缓存键中解析权限名称。
    /// </summary>
    /// <param name="key">权限授予缓存键。</param>
    /// <returns>解析出的权限名称，解析失败时返回 null。</returns>
    protected virtual string? GetPermissionNameFormCacheKeyOrNull(string key)
    {
        //TODO: 当名称为空时抛出 ex？
        return PermissionGrantCacheItem.GetPermissionNameFormCacheKeyOrNull(key);
    }
}
