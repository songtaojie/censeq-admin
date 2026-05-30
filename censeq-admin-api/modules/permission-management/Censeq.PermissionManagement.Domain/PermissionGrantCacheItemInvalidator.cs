using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.MultiTenancy;
using Censeq.PermissionManagement.Entities;

namespace Censeq.PermissionManagement;

/// <summary>
/// 权限授予缓存项失效处理器。
/// 当权限授予记录发生变化时，移除对应的分布式缓存项。
/// </summary>
public class PermissionGrantCacheItemInvalidator :ILocalEventHandler<EntityChangedEventData<PermissionGrant>>,ITransientDependency
{
    /// <summary>
    /// 当前租户
    /// </summary>
    protected ICurrentTenant CurrentTenant { get; }

    /// <summary>
    /// 分布式缓存
    /// </summary>
    protected IDistributedCache<PermissionGrantCacheItem> Cache { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="cache">分布式缓存</param>
    /// <param name="currentTenant">当前租户</param>
    public PermissionGrantCacheItemInvalidator(IDistributedCache<PermissionGrantCacheItem> cache, ICurrentTenant currentTenant)
    {
        Cache = cache;
        CurrentTenant = currentTenant;
    }

    /// <summary>
    /// 处理权限授予实体变更事件。
    /// </summary>
    /// <param name="eventData">权限授予实体变更事件数据。</param>
    /// <returns>异步任务。</returns>
    public virtual async Task HandleEventAsync(EntityChangedEventData<PermissionGrant> eventData)
    {
        var cacheKey = CalculateCacheKey(eventData.Entity.Name, eventData.Entity.ProviderName,eventData.Entity.ProviderKey);

        using (CurrentTenant.Change(eventData.Entity.TenantId))
        {
            await Cache.RemoveAsync(cacheKey, considerUow: true);
        }
    }

    /// <summary>
    /// 获取权限授予缓存键。
    /// </summary>
    /// <param name="name">权限名称</param>
    /// <param name="providerName">提供者名称</param>
    /// <param name="providerKey">提供者key</param>
    /// <returns>缓存键。</returns>
    protected virtual string CalculateCacheKey(string name, string providerName, string? providerKey)
    {
        return PermissionGrantCacheItem.CalculateCacheKey(name, providerName, providerKey);
    }
}
