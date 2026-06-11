namespace Censeq.Framework.SignalR.Dto;

/// <summary>
/// 租户在线用户缓存项，为后续分布式缓存或 Redis 扩展预留的数据结构。
/// </summary>
public class TenantOnlineUsersCacheItem
{
    /// <summary>
    /// 当前租户下的在线连接列表。
    /// </summary>
    public List<OnlineUserInfo> Users { get; set; } = [];
}
