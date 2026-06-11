using Censeq.Framework.SignalR.Dto;
using Censeq.Framework.SignalR.Extensions;
using Censeq.Framework.SignalR.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;
using Volo.Abp.Users;

namespace Censeq.Framework.SignalR.Hubs;

/// <summary>
/// 在线用户 Hub，负责登记连接、广播上下线事件、查询在线列表和执行强制下线通知。
/// </summary>
[Authorize]
[CenseqHubRoute("/hubs/online-user")]
public class OnlineUserHub : Hub<IOnlineUserClient>
{
    /// <summary>
    /// 租户分组名称前缀。
    /// </summary>
    public const string TenantGroupPrefix = "tenant:";

    private readonly IOnlineUserRegistry _registry;
    private readonly ICurrentUser _currentUser;
    private readonly ICurrentTenant _currentTenant;
    private readonly ILogger<OnlineUserHub> _logger;

    public OnlineUserHub(
        IOnlineUserRegistry registry,
        ICurrentUser currentUser,
        ICurrentTenant currentTenant,
        ILogger<OnlineUserHub> logger)
    {
        _registry = registry;
        _currentUser = currentUser;
        _currentTenant = currentTenant;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        if (!_currentUser.IsAuthenticated || !_currentUser.Id.HasValue)
        {
            Context.Abort();
            return;
        }

        var httpContext = Context.GetHttpContext();
        var info = new OnlineUserInfo
        {
            ConnectionId = Context.ConnectionId,
            UserId = _currentUser.Id.Value,
            UserName = _currentUser.UserName ?? string.Empty,
            Name = _currentUser.Name,
            TenantId = _currentTenant.Id,
            SessionId = _currentUser.FindClaim(AbpClaimTypes.SessionId)?.Value,
            Ip = httpContext?.Connection.RemoteIpAddress?.ToString(),
            UserAgent = httpContext?.Request.Headers.UserAgent.ToString(),
            ConnectedAt = DateTimeOffset.UtcNow
        };

        await _registry.AddAsync(info);
        await Groups.AddToGroupAsync(Context.ConnectionId, GroupOfTenant(info.TenantId));
        await Clients.Group(GroupOfTenant(info.TenantId)).OnlineChanged(new OnlineUserChange(info, true));

        _logger.LogInformation("SignalR user connected. UserId: {UserId}, TenantId: {TenantId}, ConnectionId: {ConnectionId}",
            info.UserId, info.TenantId, info.ConnectionId);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var info = await _registry.RemoveByConnectionAsync(Context.ConnectionId);
        if (info is not null)
        {
            await Clients.Group(GroupOfTenant(info.TenantId)).OnlineChanged(new OnlineUserChange(info, false));
            _logger.LogInformation("SignalR user disconnected. UserId: {UserId}, TenantId: {TenantId}, ConnectionId: {ConnectionId}",
                info.UserId, info.TenantId, info.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// 获取当前租户范围内的在线连接列表。
    /// </summary>
    public Task<IReadOnlyList<OnlineUserInfo>> GetOnlineList()
    {
        return _registry.GetByTenantAsync(_currentTenant.Id);
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

        await Clients.Client(input.ConnectionId).ForceOffline(reason);
        _logger.LogInformation("SignalR force offline sent. OperatorUserId: {UserId}, ConnectionId: {ConnectionId}",
            _currentUser.Id, input.ConnectionId);
    }

    /// <summary>
    /// 根据租户 ID 生成 SignalR 分组名，宿主侧使用固定 host 分组。
    /// </summary>
    public static string GroupOfTenant(Guid? tenantId)
    {
        return $"{TenantGroupPrefix}{tenantId?.ToString() ?? "host"}";
    }
}
