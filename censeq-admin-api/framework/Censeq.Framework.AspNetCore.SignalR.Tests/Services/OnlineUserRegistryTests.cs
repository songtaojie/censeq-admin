using Censeq.Framework.AspNetCore.SignalR.Dto;
using Censeq.Framework.AspNetCore.SignalR.Services;
using Microsoft.Extensions.Caching.Distributed;
using Shouldly;
using Volo.Abp.Caching;
using Xunit;

namespace Censeq.Framework.AspNetCore.SignalR.Tests.Services;

public class OnlineUserRegistryTests
{
    [Fact]
    public async Task AddAsync_ShouldMakeConnectionVisibleForTenantAndUser()
    {
        var registry = CreateRegistry();
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var info = CreateInfo("conn-1", userId, tenantId);

        await registry.AddAsync(info);

        var tenantUsers = await registry.GetByTenantAsync(tenantId);
        tenantUsers.ShouldHaveSingleItem().ConnectionId.ShouldBe("conn-1");

        var connections = await registry.GetConnectionIdsByUserAsync(userId);
        connections.ShouldBe(["conn-1"]);
    }

    [Fact]
    public async Task GetByTenantAsync_ShouldSeparateHostAndTenantConnections()
    {
        var registry = CreateRegistry();
        var tenantId = Guid.NewGuid();

        await registry.AddAsync(CreateInfo("host-conn", Guid.NewGuid(), null));
        await registry.AddAsync(CreateInfo("tenant-conn", Guid.NewGuid(), tenantId));

        var hostUsers = await registry.GetByTenantAsync(null);
        var tenantUsers = await registry.GetByTenantAsync(tenantId);

        hostUsers.ShouldHaveSingleItem().ConnectionId.ShouldBe("host-conn");
        tenantUsers.ShouldHaveSingleItem().ConnectionId.ShouldBe("tenant-conn");
    }

    [Fact]
    public async Task RemoveByConnectionAsync_ShouldReturnRemovedUserAndCleanIndexes()
    {
        var registry = CreateRegistry();
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        await registry.AddAsync(CreateInfo("conn-1", userId, tenantId));

        var removed = await registry.RemoveByConnectionAsync("conn-1");

        removed.ShouldNotBeNull();
        removed.ConnectionId.ShouldBe("conn-1");
        (await registry.GetByTenantAsync(tenantId)).ShouldBeEmpty();
        (await registry.GetConnectionIdsByUserAsync(userId)).ShouldBeEmpty();
    }

    private static OnlineUserRegistry CreateRegistry()
    {
        return new OnlineUserRegistry(
            new InMemoryDistributedCache<TenantOnlineUsersCacheItem>(),
            new InMemoryDistributedCache<OnlineUserInfo>(),
            new InMemoryDistributedCache<UserOnlineConnectionsCacheItem>());
    }

    private static OnlineUserInfo CreateInfo(string connectionId, Guid userId, Guid? tenantId)
    {
        return new OnlineUserInfo
        {
            ConnectionId = connectionId,
            UserId = userId,
            UserName = $"user-{connectionId}",
            Name = $"User {connectionId}",
            TenantId = tenantId,
            SessionId = $"session-{connectionId}",
            Ip = "127.0.0.1",
            UserAgent = "test-agent",
            ConnectedAt = DateTimeOffset.UtcNow
        };
    }

    private sealed class InMemoryDistributedCache<TCacheItem> : IDistributedCache<TCacheItem>
        where TCacheItem : class
    {
        private readonly Dictionary<string, TCacheItem> _items = [];

        public IDistributedCache<TCacheItem, string> InternalCache => this;

        public TCacheItem? Get(string key, bool? hideErrors = null, bool considerUow = false)
        {
            return _items.GetValueOrDefault(key);
        }

        public Task<TCacheItem?> GetAsync(
            string key,
            bool? hideErrors = null,
            bool considerUow = false,
            CancellationToken token = default)
        {
            return Task.FromResult(Get(key, hideErrors, considerUow));
        }

        public KeyValuePair<string, TCacheItem?>[] GetMany(
            IEnumerable<string> keys,
            bool? hideErrors = null,
            bool considerUow = false)
        {
            return keys.Select(key => new KeyValuePair<string, TCacheItem?>(key, Get(key, hideErrors, considerUow))).ToArray();
        }

        public Task<KeyValuePair<string, TCacheItem?>[]> GetManyAsync(
            IEnumerable<string> keys,
            bool? hideErrors = null,
            bool considerUow = false,
            CancellationToken token = default)
        {
            return Task.FromResult(GetMany(keys, hideErrors, considerUow));
        }

        public TCacheItem GetOrAdd(
            string key,
            Func<TCacheItem> factory,
            Func<DistributedCacheEntryOptions>? optionsFactory = null,
            bool? hideErrors = null,
            bool considerUow = false)
        {
            if (!_items.TryGetValue(key, out var value))
            {
                value = factory();
                _items[key] = value;
            }

            return value;
        }

        public async Task<TCacheItem?> GetOrAddAsync(
            string key,
            Func<Task<TCacheItem>> factory,
            Func<DistributedCacheEntryOptions>? optionsFactory = null,
            bool? hideErrors = null,
            bool considerUow = false,
            CancellationToken token = default)
        {
            if (!_items.TryGetValue(key, out var value))
            {
                value = await factory();
                _items[key] = value;
            }

            return value;
        }

        public KeyValuePair<string, TCacheItem?>[] GetOrAddMany(
            IEnumerable<string> keys,
            Func<IEnumerable<string>, List<KeyValuePair<string, TCacheItem>>> factory,
            Func<DistributedCacheEntryOptions>? optionsFactory = null,
            bool? hideErrors = null,
            bool considerUow = false)
        {
            var missingKeys = keys.Where(key => !_items.ContainsKey(key)).ToList();
            foreach (var item in factory(missingKeys))
            {
                _items[item.Key] = item.Value;
            }

            return keys.Select(key => new KeyValuePair<string, TCacheItem?>(key, _items[key])).ToArray();
        }

        public async Task<KeyValuePair<string, TCacheItem?>[]> GetOrAddManyAsync(
            IEnumerable<string> keys,
            Func<IEnumerable<string>, Task<List<KeyValuePair<string, TCacheItem>>>> factory,
            Func<DistributedCacheEntryOptions>? optionsFactory = null,
            bool? hideErrors = null,
            bool considerUow = false,
            CancellationToken token = default)
        {
            var missingKeys = keys.Where(key => !_items.ContainsKey(key)).ToList();
            foreach (var item in await factory(missingKeys))
            {
                _items[item.Key] = item.Value;
            }

            return keys.Select(key => new KeyValuePair<string, TCacheItem?>(key, _items[key])).ToArray();
        }

        public void Set(
            string key,
            TCacheItem value,
            DistributedCacheEntryOptions? options = null,
            bool? hideErrors = null,
            bool considerUow = false)
        {
            _items[key] = value;
        }

        public Task SetAsync(
            string key,
            TCacheItem value,
            DistributedCacheEntryOptions? options = null,
            bool? hideErrors = null,
            bool considerUow = false,
            CancellationToken token = default)
        {
            Set(key, value, options, hideErrors, considerUow);
            return Task.CompletedTask;
        }

        public void SetMany(
            IEnumerable<KeyValuePair<string, TCacheItem>> items,
            DistributedCacheEntryOptions? options = null,
            bool? hideErrors = null,
            bool considerUow = false)
        {
            foreach (var item in items)
            {
                _items[item.Key] = item.Value;
            }
        }

        public Task SetManyAsync(
            IEnumerable<KeyValuePair<string, TCacheItem>> items,
            DistributedCacheEntryOptions? options = null,
            bool? hideErrors = null,
            bool considerUow = false,
            CancellationToken token = default)
        {
            SetMany(items, options, hideErrors, considerUow);
            return Task.CompletedTask;
        }

        public void Refresh(string key, bool? hideErrors = null)
        {
        }

        public Task RefreshAsync(string key, bool? hideErrors = null, CancellationToken token = default)
        {
            return Task.CompletedTask;
        }

        public void RefreshMany(IEnumerable<string> keys, bool? hideErrors = null)
        {
        }

        public Task RefreshManyAsync(IEnumerable<string> keys, bool? hideErrors = null, CancellationToken token = default)
        {
            return Task.CompletedTask;
        }

        public void Remove(string key, bool? hideErrors = null, bool considerUow = false)
        {
            _items.Remove(key);
        }

        public Task RemoveAsync(
            string key,
            bool? hideErrors = null,
            bool considerUow = false,
            CancellationToken token = default)
        {
            Remove(key, hideErrors, considerUow);
            return Task.CompletedTask;
        }

        public void RemoveMany(IEnumerable<string> keys, bool? hideErrors = null, bool considerUow = false)
        {
            foreach (var key in keys)
            {
                _items.Remove(key);
            }
        }

        public Task RemoveManyAsync(
            IEnumerable<string> keys,
            bool? hideErrors = null,
            bool considerUow = false,
            CancellationToken token = default)
        {
            RemoveMany(keys, hideErrors, considerUow);
            return Task.CompletedTask;
        }
    }
}
