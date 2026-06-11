using Censeq.Framework.AspNetCore.SignalR.Dto;

namespace Censeq.Framework.AspNetCore.SignalR.Services;

/// <summary>
/// 面向业务层的实时通知发送服务。
/// </summary>
public interface INotificationSender
{
    /// <summary>
    /// 向所有已连接客户端发送通知。
    /// </summary>
    Task SendToAllAsync(NotificationMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// 向指定租户内的所有客户端发送通知，租户为空表示宿主侧。
    /// </summary>
    Task SendToTenantAsync(Guid? tenantId, NotificationMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// 向指定用户的全部在线连接发送通知。
    /// </summary>
    Task SendToUserAsync(Guid userId, NotificationMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// 向多个用户的全部在线连接发送通知。
    /// </summary>
    Task SendToUsersAsync(IEnumerable<Guid> userIds, NotificationMessage message, CancellationToken cancellationToken = default);
}