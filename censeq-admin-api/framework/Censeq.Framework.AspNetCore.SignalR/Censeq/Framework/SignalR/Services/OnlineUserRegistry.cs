using Censeq.Framework.AspNetCore.SignalR.Dto;
using Microsoft.Extensions.Caching.Distributed;
using Volo.Abp.Caching;

namespace Censeq.Framework.AspNetCore.SignalR.Services;

/// <summary>
/// 基于分布式缓存的在线用户连接注册表。
/// </summary>
public class OnlineUserRegistry : IOnlineUserRegistry
{
    private static readonly SemaphoreSlim SyncLock = new(1, 1);

    private readonly IDistributedCache<TenantOnlineUsersCacheItem> _tenantCache;
    private readonly IDistributedCache<OnlineUserInfo> _connectionCache;
    private readonly IDistributedCache<UserOnlineConnectionsCacheItem> _userConnectionsCache;

    public OnlineUserRegistry(
        IDistributedCache<TenantOnlineUsersCacheItem> tenantCache,
        IDistributedCache<OnlineUserInfo> connectionCache,
        IDistributedCache<UserOnlineConnectionsCacheItem> userConnectionsCache)
    {
        _tenantCache = tenantCache;
        _connectionCache = connectionCache;
        _userConnectionsCache = userConnectionsCache;
    }

    public async Task AddAsync(OnlineUserInfo info)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(info.ConnectionId);

        await SyncLock.WaitAsync();
        try
        {
            var tenantKey = GetTenantKey(info.TenantId);
            var tenantItem = await _tenantCache.GetAsync(tenantKey) ?? new TenantOnlineUsersCacheItem();
            tenantItem.Users.RemoveAll(x => x.ConnectionId == info.ConnectionId);
            tenantItem.Users.Add(info);

            var userKey = GetUserKey(info.UserId);
            var userItem = await _userConnectionsCache.GetAsync(userKey) ?? new UserOnlineConnectionsCacheItem();
            userItem.ConnectionIds.Remove(info.ConnectionId);
            userItem.ConnectionIds.Add(info.ConnectionId);

            await _tenantCache.SetAsync(tenantKey, tenantItem, CreateEntryOptions());
            await _connectionCache.SetAsync(GetConnectionKey(info.ConnectionId), info, CreateEntryOptions());
            await _userConnectionsCache.SetAsync(userKey, userItem, CreateEntryOptions());
        }
        finally
        {
            SyncLock.Release();
        }
    }

    public async Task<OnlineUserInfo?> RemoveByConnectionAsync(string connectionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionId);

        await SyncLock.WaitAsync();
        try
        {
            var info = await _connectionCache.GetAsync(GetConnectionKey(connectionId));
            if (info is null)
            {
                return null;
            }

            var tenantKey = GetTenantKey(info.TenantId);
            var tenantItem = await _tenantCache.GetAsync(tenantKey) ?? new TenantOnlineUsersCacheItem();
            tenantItem.Users.RemoveAll(x => x.ConnectionId == connectionId);

            var userKey = GetUserKey(info.UserId);
            var userItem = await _userConnectionsCache.GetAsync(userKey) ?? new UserOnlineConnectionsCacheItem();
            userItem.ConnectionIds.Remove(connectionId);

            await _tenantCache.SetAsync(tenantKey, tenantItem, CreateEntryOptions());
            await _connectionCache.RemoveAsync(GetConnectionKey(connectionId));

            if (userItem.ConnectionIds.Count == 0)
            {
                await _userConnectionsCache.RemoveAsync(userKey);
            }
            else
            {
                await _userConnectionsCache.SetAsync(userKey, userItem, CreateEntryOptions());
            }

            return info;
        }
        finally
        {
            SyncLock.Release();
        }
    }

    public async Task<IReadOnlyList<OnlineUserInfo>> GetByTenantAsync(Guid? tenantId)
    {
        var tenantItem = await _tenantCache.GetAsync(GetTenantKey(tenantId));
        return (tenantItem?.Users ?? [])
            .OrderBy(x => x.ConnectedAt)
            .ToList();
    }

    public async Task<IReadOnlyList<string>> GetConnectionIdsByUserAsync(Guid userId)
    {
        var userItem = await _userConnectionsCache.GetAsync(GetUserKey(userId));
        return userItem?.ConnectionIds.Distinct().ToList() ?? [];
    }

    private static string GetTenantKey(Guid? tenantId)
    {
        return $"signalr:online:tenant:{tenantId?.ToString() ?? "host"}";
    }

    private static string GetConnectionKey(string connectionId)
    {
        return $"signalr:online:conn:{connectionId}";
    }

    private static string GetUserKey(Guid userId)
    {
        return $"signalr:online:user:{userId}";
    }

    private static DistributedCacheEntryOptions CreateEntryOptions()
    {
        return new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
        };
    }
}
