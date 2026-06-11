using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Volo.Abp.Security.Claims;

namespace Censeq.Framework.SignalR;

/// <summary>
/// 将 ABP 当前登录用户标识映射为 SignalR 的 UserIdentifier，用于按用户推送消息。
/// </summary>
public sealed class AbpUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
               ?? connection.User?.FindFirst(AbpClaimTypes.UserId)?.Value;
    }
}
