using System.Collections.Concurrent;
using Censeq.Framework.SignalR.Dto;

namespace Censeq.Framework.SignalR.Services;

/// <summary>
/// 基于内存字典的在线用户注册表实现，适用于单实例部署场景。
/// </summary>
public class OnlineUserRegistry : IOnlineUserRegistry
{
    private readonly ConcurrentDictionary<string, OnlineUserInfo> _connections = new();

    public Task AddAsync(OnlineUserInfo info)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(info.ConnectionId);
        _connections[info.ConnectionId] = info;
        return Task.CompletedTask;
    }

    public Task<OnlineUserInfo?> RemoveByConnectionAsync(string connectionId)
    {
        _connections.TryRemove(connectionId, out var info);
        return Task.FromResult(info);
    }

    public Task<IReadOnlyList<OnlineUserInfo>> GetByTenantAsync(Guid? tenantId)
    {
        IReadOnlyList<OnlineUserInfo> users = _connections.Values
            .Where(x => x.TenantId == tenantId)
            .OrderBy(x => x.ConnectedAt)
            .ToList();

        return Task.FromResult(users);
    }

    public Task<IReadOnlyList<string>> GetConnectionIdsByUserAsync(Guid userId)
    {
        IReadOnlyList<string> connectionIds = _connections.Values
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.ConnectedAt)
            .Select(x => x.ConnectionId)
            .ToList();

        return Task.FromResult(connectionIds);
    }
}
