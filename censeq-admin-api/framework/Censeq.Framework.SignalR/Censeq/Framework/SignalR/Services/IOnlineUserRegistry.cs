using Censeq.Framework.SignalR.Dto;

namespace Censeq.Framework.SignalR.Services;

/// <summary>
/// 在线用户连接注册表，抽象连接登记、移除和查询能力。
/// </summary>
public interface IOnlineUserRegistry
{
    /// <summary>
    /// 登记或更新一条在线连接。
    /// </summary>
    Task AddAsync(OnlineUserInfo info);

    /// <summary>
    /// 按连接 ID 移除在线连接，并返回被移除的连接信息。
    /// </summary>
    Task<OnlineUserInfo?> RemoveByConnectionAsync(string connectionId);

    /// <summary>
    /// 获取指定租户范围内的在线连接列表。
    /// </summary>
    Task<IReadOnlyList<OnlineUserInfo>> GetByTenantAsync(Guid? tenantId);

    /// <summary>
    /// 获取指定用户当前所有在线连接 ID。
    /// </summary>
    Task<IReadOnlyList<string>> GetConnectionIdsByUserAsync(Guid userId);
}
