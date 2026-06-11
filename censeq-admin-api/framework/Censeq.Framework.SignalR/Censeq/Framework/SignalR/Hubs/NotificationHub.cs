using Censeq.Framework.SignalR.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Volo.Abp.MultiTenancy;

namespace Censeq.Framework.SignalR.Hubs;

/// <summary>
/// 实时通知 Hub，用于维护租户分组并向客户端推送业务通知。
/// </summary>
[Authorize]
[CenseqHubRoute("/hubs/notification")]
public class NotificationHub : Hub<INotificationClient>
{
    private readonly ICurrentTenant _currentTenant;

    public NotificationHub(ICurrentTenant currentTenant)
    {
        _currentTenant = currentTenant;
    }

    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, OnlineUserHub.GroupOfTenant(_currentTenant.Id));
        await base.OnConnectedAsync();
    }
}
