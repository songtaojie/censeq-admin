namespace Censeq.Framework.AspNetCore.SignalR.Dto;

/// <summary>
/// 租户在线用户缓存项。
/// </summary>
public class TenantOnlineUsersCacheItem
{
    /// <summary>
    /// 当前租户下的在线连接列表。
    /// </summary>
    public List<OnlineUserInfo> Users { get; set; } = [];
}