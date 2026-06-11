namespace Censeq.Framework.AspNetCore.SignalR.Dto;

/// <summary>
/// 用户在线连接缓存项，用于记录指定用户当前所有 SignalR 连接。
/// </summary>
public class UserOnlineConnectionsCacheItem
{
    /// <summary>
    /// 用户当前在线的 SignalR 连接 ID 列表。
    /// </summary>
    public List<string> ConnectionIds { get; set; } = [];
}
