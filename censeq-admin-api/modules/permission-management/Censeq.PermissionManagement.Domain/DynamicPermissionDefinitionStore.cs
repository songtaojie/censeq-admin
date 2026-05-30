using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Censeq.PermissionManagement;
using Volo.Abp;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DistributedLocking;
using Volo.Abp.Threading;

namespace Censeq.PermissionManagement;

/// <summary>
/// 动态权限定义存储。
/// 从数据库读取运行时权限定义，并使用进程内缓存和分布式标记控制刷新。
/// </summary>
[Dependency(ReplaceServices = true)]
public class DynamicPermissionDefinitionStore : IDynamicPermissionDefinitionStore, ITransientDependency
{
    /// <summary>
    /// 权限组仓储。
    /// </summary>
    protected IPermissionGroupRepository PermissionGroupRepository { get; }

    /// <summary>
    /// 权限定义记录仓储。
    /// </summary>
    protected IPermissionDefinitionRecordRepository PermissionRepository { get; }

    /// <summary>
    /// 权限定义序列化器。
    /// </summary>
    protected IPermissionDefinitionSerializer PermissionDefinitionSerializer { get; }

    /// <summary>
    /// 动态权限定义的进程内缓存。
    /// </summary>
    protected IDynamicPermissionDefinitionStoreInMemoryCache StoreCache { get; }

    /// <summary>
    /// 分布式缓存。
    /// </summary>
    protected IDistributedCache DistributedCache { get; }

    /// <summary>
    /// 分布式锁。
    /// </summary>
    protected IAbpDistributedLock DistributedLock { get; }

    /// <summary>
    /// 权限管理选项。
    /// </summary>
    public PermissionManagementOptions PermissionManagementOptions { get; }

    /// <summary>
    /// 分布式缓存选项。
    /// </summary>
    protected AbpDistributedCacheOptions CacheOptions { get; }


    /// <summary>
    /// 初始化动态权限定义存储。
    /// </summary>
    /// <param name="permissionGroupRepository">权限组仓储。</param>
    /// <param name="permissionRepository">权限定义记录仓储。</param>
    /// <param name="permissionDefinitionSerializer">权限定义序列化器。</param>
    /// <param name="storeCache">进程内权限定义缓存。</param>
    /// <param name="distributedCache">分布式缓存。</param>
    /// <param name="cacheOptions">分布式缓存配置。</param>
    /// <param name="permissionManagementOptions">权限管理配置。</param>
    /// <param name="distributedLock">分布式锁。</param>
    public DynamicPermissionDefinitionStore(
        IPermissionGroupRepository permissionGroupRepository,
        IPermissionDefinitionRecordRepository permissionRepository,
        IPermissionDefinitionSerializer permissionDefinitionSerializer,
        IDynamicPermissionDefinitionStoreInMemoryCache storeCache,
        IDistributedCache distributedCache,
        IOptions<AbpDistributedCacheOptions> cacheOptions,
        IOptions<PermissionManagementOptions> permissionManagementOptions,
        IAbpDistributedLock distributedLock)
    {
        PermissionGroupRepository = permissionGroupRepository;
        PermissionRepository = permissionRepository;
        PermissionDefinitionSerializer = permissionDefinitionSerializer;
        StoreCache = storeCache;
        DistributedCache = distributedCache;
        DistributedLock = distributedLock;
        PermissionManagementOptions = permissionManagementOptions.Value;
        CacheOptions = cacheOptions.Value;
    }

    /// <summary>
    /// 根据名称获取动态权限定义。
    /// </summary>
    /// <param name="name">权限名称。</param>
    /// <returns>权限定义不存在或动态存储关闭时返回 null。</returns>
    public virtual async Task<PermissionDefinition?> GetOrNullAsync(string name)
    {
        if (!PermissionManagementOptions.IsDynamicPermissionStoreEnabled)
        {
            return null;
        }

        using (await StoreCache.SyncSemaphore.LockAsync())
        {
            await EnsureCacheIsUptoDateAsync();
            return StoreCache.GetPermissionOrNull(name);
        }
    }

    /// <summary>
    /// 获取全部动态权限定义。
    /// </summary>
    /// <returns>动态权限定义列表。</returns>
    public virtual async Task<IReadOnlyList<PermissionDefinition>> GetPermissionsAsync()
    {
        if (!PermissionManagementOptions.IsDynamicPermissionStoreEnabled)
        {
            return Array.Empty<PermissionDefinition>();
        }

        using (await StoreCache.SyncSemaphore.LockAsync())
        {
            await EnsureCacheIsUptoDateAsync();
            return StoreCache.GetPermissions().ToImmutableList();
        }
    }

    /// <summary>
    /// 获取全部动态权限组定义。
    /// </summary>
    /// <returns>动态权限组定义列表。</returns>
    public virtual async Task<IReadOnlyList<PermissionGroupDefinition>> GetGroupsAsync()
    {
        if (!PermissionManagementOptions.IsDynamicPermissionStoreEnabled)
        {
            return [];
        }

        using (await StoreCache.SyncSemaphore.LockAsync())
        {
            await EnsureCacheIsUptoDateAsync();
            return StoreCache.GetGroups().ToImmutableList();
        }
    }

    /// <summary>
    /// 确保进程内缓存及时更新。
    /// 通过分布式缓存中的标记判断其他实例是否已经写入新的权限定义。
    /// </summary>
    /// <returns>异步任务。</returns>
    protected virtual async Task EnsureCacheIsUptoDateAsync()
    {
        if (StoreCache.LastCheckTime.HasValue &&
            DateTime.Now.Subtract(StoreCache.LastCheckTime.Value).TotalSeconds < 30)
        {
            /* 为了优化缓存读取，短时间内重复访问时不立即检查分布式标记。 */
            return;
        }

        var stampInDistributedCache = await GetOrSetStampInDistributedCache();

        if (stampInDistributedCache == StoreCache.CacheStamp)
        {
            StoreCache.LastCheckTime = DateTime.Now;
            return;
        }

        await UpdateInMemoryStoreCache();

        StoreCache.CacheStamp = stampInDistributedCache;
        StoreCache.LastCheckTime = DateTime.Now;
    }

    /// <summary>
    /// 从数据库重新加载权限组和权限定义，并刷新进程内缓存。
    /// </summary>
    /// <returns>异步任务。</returns>
    protected virtual async Task UpdateInMemoryStoreCache()
    {
        var permissionGroupRecords = await PermissionGroupRepository.GetListAsync();
        var permissionRecords = await PermissionRepository.GetListAsync();

        await StoreCache.FillAsync(permissionGroupRecords, permissionRecords);
    }

    /// <summary>
    /// 获取或初始化分布式缓存中的公共权限定义标记。
    /// </summary>
    /// <returns>当前权限定义缓存标记。</returns>
    /// <exception cref="AbpException">无法获取初始化标记所需的分布式锁时抛出。</exception>
    protected virtual async Task<string> GetOrSetStampInDistributedCache()
    {
        var cacheKey = GetCommonStampCacheKey();

        var stampInDistributedCache = await DistributedCache.GetStringAsync(cacheKey);
        if (stampInDistributedCache != null)
        {
            return stampInDistributedCache;
        }

        await using (var commonLockHandle = await DistributedLock.TryAcquireAsync(GetCommonDistributedLockKey(), TimeSpan.FromMinutes(1)))
        {
            if (commonLockHandle == null)
            {
                /* This request will fail */
                throw new AbpException("无法获取用于权限定义缓存初始化的分布式锁。");
            }

            stampInDistributedCache = await DistributedCache.GetStringAsync(cacheKey);
            if (stampInDistributedCache != null)
            {
                return stampInDistributedCache;
            }

            stampInDistributedCache = Guid.NewGuid().ToString();

            await DistributedCache.SetStringAsync(cacheKey,
                stampInDistributedCache,
                new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromDays(30) //TODO: Make it configurable?
                }
            );
        }

        return stampInDistributedCache;
    }

    /// <summary>
    /// 获取公共权限定义缓存标记的缓存键。
    /// </summary>
    /// <returns>缓存键。</returns>
    protected virtual string GetCommonStampCacheKey()
    {
        return $"{CacheOptions.KeyPrefix}:InMemory:PermissionCacheStamp";
    }

    /// <summary>
    /// 获取刷新权限定义缓存时使用的公共分布式锁键。
    /// </summary>
    /// <returns>分布式锁键。</returns>
    protected virtual string GetCommonDistributedLockKey()
    {
        return $"{CacheOptions.KeyPrefix}:Permission:UpdateLock";
    }
}
