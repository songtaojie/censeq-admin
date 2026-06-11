using Censeq.Framework.SignalR.Dto;

namespace Censeq.Framework.SignalR.Hubs;

/// <summary>
/// 在线用户 Hub 调用前端客户端时使用的强类型契约。
/// </summary>
public interface IOnlineUserClient
{
    /// <summary>
    /// 广播单个用户连接的上线或离线事件。
    /// </summary>
    Task OnlineChanged(OnlineUserChange change);

    /// <summary>
    /// 推送当前租户的在线用户完整列表。
    /// </summary>
    Task OnlineList(IReadOnlyList<OnlineUserInfo> list);

    /// <summary>
    /// 通知指定连接执行强制下线处理。
    /// </summary>
    Task ForceOffline(string reason);
}
