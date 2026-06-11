using Microsoft.AspNetCore.Authorization;
using Volo.Abp.AspNetCore.SignalR;

namespace Censeq.Framework.AspNetCore.SignalR.Hubs;

/// <summary>
/// 实时通知 Hub，用于维护租户分组并向客户端推送业务通知。
/// </summary>
[Authorize]
[HubRoute("/hubs/notification")]
public class NotificationHub : AbpHub<INotificationClient>
{
    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, OnlineUserHub.GroupOfTenant(CurrentTenant.Id));
        await base.OnConnectedAsync();
    }
}