using Starshine.Admin.Entities;
using System;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Local;
using Volo.Abp.MultiTenancy;

namespace Starshine.Admin;

/// <summary>
/// 租户配置缓存项验证器
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="cache"></param>
[LocalEventHandlerOrder(-1)]
public class TenantConfigurationCacheItemInvalidator(IDistributedCache<TenantConfigurationCacheItem> cache) :
    ILocalEventHandler<EntityChangedEventData<Tenant>>,
    ILocalEventHandler<TenantChangedEvent>,
    ITransientDependency
{
    /// <summary>
    /// 缓存服务
    /// </summary>
    protected IDistributedCache<TenantConfigurationCacheItem> Cache { get; } = cache;

    /// <summary>
    /// 处理事件
    /// </summary>
    /// <param name="eventData"></param>
    /// <returns></returns>
    public virtual async Task HandleEventAsync(EntityChangedEventData<Tenant> eventData)
    {
        if (eventData is EntityCreatedEventData<Tenant>)
        {
            return;
        }

        await ClearCacheAsync(eventData.Entity.Id, eventData.Entity.NormalizedName);
    }

    /// <summary>
    /// 处理事件
    /// </summary>
    /// <param name="eventData"></param>
    /// <returns></returns>
    public virtual async Task HandleEventAsync(TenantChangedEvent eventData)
    {
        await ClearCacheAsync(eventData.Id, eventData.NormalizedName);
    }

    /// <summary>
    /// 清除缓存
    /// </summary>
    /// <param name="id"></param>
    /// <param name="normalizedName"></param>
    /// <returns></returns>
    protected virtual async Task ClearCacheAsync(Guid? id, string? normalizedName)
    {
        await Cache.RemoveManyAsync(
            [
                TenantConfigurationCacheItem.CalculateCacheKey(id, null),
                TenantConfigurationCacheItem.CalculateCacheKey(null, normalizedName),
                TenantConfigurationCacheItem.CalculateCacheKey(id, normalizedName),
            ], considerUow: true);
    }
}