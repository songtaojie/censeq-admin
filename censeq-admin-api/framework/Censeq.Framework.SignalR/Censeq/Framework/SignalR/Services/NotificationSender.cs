using Censeq.Framework.SignalR.Dto;
using Censeq.Framework.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Censeq.Framework.SignalR.Services;

/// <summary>
/// 基于 SignalR HubContext 的实时通知发送服务实现。
/// </summary>
public class NotificationSender : INotificationSender
{
    private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;
    private readonly ILogger<NotificationSender> _logger;

    public NotificationSender(
        IHubContext<NotificationHub, INotificationClient> hubContext,
        ILogger<NotificationSender> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task SendToAllAsync(NotificationMessage message, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.All.ReceiveMessage(message);
        _logger.LogInformation("SignalR notification sent to all. MessageId: {MessageId}", message.Id);
    }

    public async Task SendToTenantAsync(Guid? tenantId, NotificationMessage message, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(OnlineUserHub.GroupOfTenant(tenantId)).ReceiveMessage(message);
        _logger.LogInformation("SignalR notification sent to tenant. TenantId: {TenantId}, MessageId: {MessageId}", tenantId, message.Id);
    }

    public async Task SendToUserAsync(Guid userId, NotificationMessage message, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.User(userId.ToString()).ReceiveMessage(message);
        _logger.LogInformation("SignalR notification sent to user. UserId: {UserId}, MessageId: {MessageId}", userId, message.Id);
    }

    public async Task SendToUsersAsync(IEnumerable<Guid> userIds, NotificationMessage message, CancellationToken cancellationToken = default)
    {
        var identifiers = userIds.Select(x => x.ToString()).Distinct().ToList();
        if (identifiers.Count == 0)
        {
            return;
        }

        await _hubContext.Clients.Users(identifiers).ReceiveMessage(message);
        _logger.LogInformation("SignalR notification sent to users. UserCount: {UserCount}, MessageId: {MessageId}", identifiers.Count, message.Id);
    }
}
