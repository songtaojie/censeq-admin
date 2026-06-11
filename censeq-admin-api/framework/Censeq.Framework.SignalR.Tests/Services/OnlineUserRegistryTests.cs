using Censeq.Framework.SignalR.Dto;
using Censeq.Framework.SignalR.Services;
using Shouldly;
using Xunit;

namespace Censeq.Framework.SignalR.Tests.Services;

/// <summary>
/// 在线用户注册表的连接登记、租户隔离和移除行为测试。
/// </summary>
public class OnlineUserRegistryTests
{
    [Fact]
    public async Task AddAsync_ShouldMakeConnectionVisibleForTenantAndUser()
    {
        var registry = new OnlineUserRegistry();
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
        var registry = new OnlineUserRegistry();
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
        var registry = new OnlineUserRegistry();
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        await registry.AddAsync(CreateInfo("conn-1", userId, tenantId));

        var removed = await registry.RemoveByConnectionAsync("conn-1");

        removed.ShouldNotBeNull();
        removed.ConnectionId.ShouldBe("conn-1");
        (await registry.GetByTenantAsync(tenantId)).ShouldBeEmpty();
        (await registry.GetConnectionIdsByUserAsync(userId)).ShouldBeEmpty();
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
}
