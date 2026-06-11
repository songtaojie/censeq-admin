using Censeq.Framework.AspNetCore.SignalR.Dto;
using Censeq.Framework.AspNetCore.SignalR.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.Security.Claims;

namespace Censeq.Framework.AspNetCore.SignalR.Hubs;

/// <summary>
/// 在线用户 Hub，负责登记连接、广播上下线事件、查询在线列表和发送强制下线通知。
/// </summary>
[Authorize]
[HubRoute("/hubs/online-user")]
public class OnlineUserHub : AbpHub<IOnlineUserClient>
{
    /// <summary>
    /// 租户分组名称前缀。
    /// </summary>
    public const string TenantGroupPrefix = "tenant:";

    private readonly IOnlineUserRegistry _registry;
    private readonly IHubContext<OnlineUserHub, IOnlineUserClient> _hubContext;

    public OnlineUserHub(
        IOnlineUserRegistry registry,
        IHubContext<OnlineUserHub, IOnlineUserClient> hubContext)
    {
        _registry = registry;
        _hubContext = hubContext;
    }

    public override async Task OnConnectedAsync()
    {
        if (!CurrentUser.IsAuthenticated || !CurrentUser.Id.HasValue)
        {
            Context.Abort();
            return;
        }

        var httpContext = Context.GetHttpContext();
        var info = new OnlineUserInfo
        {
            ConnectionId = Context.ConnectionId,
            UserId = CurrentUser.Id.Value,
            UserName = CurrentUser.UserName ?? string.Empty,
            Name = CurrentUser.Name,
            TenantId = CurrentTenant.Id,
            SessionId = CurrentUser.FindClaim(AbpClaimTypes.SessionId)?.Value,
            Ip = httpContext?.GetRemoteIpAddressToIPv4(),
            UserAgent = httpContext?.Request.Headers.UserAgent.ToString(),
            ConnectedAt = DateTimeOffset.UtcNow
        };

        await _registry.AddAsync(info);
        await Groups.AddToGroupAsync(Context.ConnectionId, GroupOfTenant(info.TenantId));
        await _hubContext.Clients.Group(GroupOfTenant(info.TenantId)).OnlineChanged(new OnlineUserChange(info, true));

        Logger.LogInformation("SignalR user connected. UserId: {UserId}, TenantId: {TenantId}, ConnectionId: {ConnectionId}",
            info.UserId, info.TenantId, info.ConnectionId);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var info = await _registry.RemoveByConnectionAsync(Context.ConnectionId);
        if (info is not null)
        {
            await _hubContext.Clients.Group(GroupOfTenant(info.TenantId)).OnlineChanged(new OnlineUserChange(info, false));
            Logger.LogInformation("SignalR user disconnected. UserId: {UserId}, TenantId: {TenantId}, ConnectionId: {ConnectionId}",
                info.UserId, info.TenantId, info.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// 获取当前租户范围内的在线连接列表。
    /// </summary>
    public Task<IReadOnlyList<OnlineUserInfo>> GetOnlineList()
    {
        return _registry.GetByTenantAsync(CurrentTenant.Id);
    }

    /// <summary>
    /// 向指定连接发送强制下线通知。
    /// </summary>
    [Authorize("CenseqIdentity.Sessions.Revoke")]
    public async Task ForceOffline(ForceOfflineInput input)
    {
        var reason = string.IsNullOrWhiteSpace(input.Reason)
            ? "您已被管理员强制下线"
            : input.Reason;

        await _hubContext.Clients.Client(input.ConnectionId).ForceOffline(reason);
        Logger.LogInformation("SignalR force offline sent. OperatorUserId: {UserId}, ConnectionId: {ConnectionId}",
            CurrentUser.Id, input.ConnectionId);
    }

    /// <summary>
    /// 根据租户 ID 生成 SignalR 分组名，宿主侧使用固定 host 分组。
    /// </summary>
    public static string GroupOfTenant(Guid? tenantId)
    {
        return $"{TenantGroupPrefix}{tenantId?.ToString() ?? "host"}";
    }
}