using Censeq.Framework.SignalR.Dto;

namespace Censeq.Framework.SignalR.Hubs;

/// <summary>
/// 通知 Hub 调用前端客户端时使用的强类型契约。
/// </summary>
public interface INotificationClient
{
    /// <summary>
    /// 接收服务端推送的实时通知消息。
    /// </summary>
    Task ReceiveMessage(NotificationMessage message);
}
