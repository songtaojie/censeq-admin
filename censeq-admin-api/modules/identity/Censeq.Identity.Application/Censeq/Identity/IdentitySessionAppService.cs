using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Censeq.Identity.Entities;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Security.Claims;

namespace Censeq.Identity;

/// <summary>
/// 用户会话应用服务
/// </summary>
[Authorize]
public class IdentitySessionAppService : IdentityAppServiceBase, IIdentitySessionAppService
{
    protected IdentitySessionManager SessionManager { get; }

    public IdentitySessionAppService(IdentitySessionManager sessionManager)
    {
        SessionManager = sessionManager;
    }

    /// <summary>
    /// 获取当前用户的所有会话
    /// </summary>
    public virtual async Task<List<IdentitySessionDto>> GetMySessionsAsync()
    {
        var sessions = await SessionManager.GetListAsync(CurrentUser.Id!.Value);
        var currentSessionId = CurrentUser.FindClaim(AbpClaimTypes.SessionId)?.Value;

        var result = ObjectMapper.Map<List<IdentitySession>, List<IdentitySessionDto>>(sessions);

        // 标记当前会话
        foreach (var session in result)
        {
            session.IsCurrentSession = session.SessionId == currentSessionId;
        }

        return result;
    }

    /// <summary>
    /// 获取指定用户的所有会话（需要管理权限）
    /// </summary>
    [Authorize(IdentityPermissions.Sessions.Manage)]
    public virtual async Task<List<IdentitySessionDto>> GetUserSessionsAsync(Guid userId)
    {
        var sessions = await SessionManager.GetListAsync(userId);
        return ObjectMapper.Map<List<IdentitySession>, List<IdentitySessionDto>>(sessions);
    }

    /// <summary>
    /// 终止当前用户的指定会话
    /// </summary>
    public virtual async Task RevokeMySessionAsync(string sessionId)
    {
        // 验证是否是自己的会话
        var session = await SessionManager.FindAsync(sessionId);
        if (session == null)
        {
            return;
        }

        if (session.UserId != CurrentUser.Id!.Value)
        {
            throw new Volo.Abp.Authorization.AbpAuthorizationException("You can only revoke your own sessions");
        }

        await SessionManager.DeleteAsync(sessionId);
    }

    /// <summary>
    /// 终止指定用户的指定会话（需要管理权限）
    /// </summary>
    [Authorize(IdentityPermissions.Sessions.Revoke)]
    public virtual async Task RevokeUserSessionAsync(Guid userId, string sessionId)
    {
        var session = await SessionManager.FindAsync(sessionId);
        if (session == null)
        {
            return;
        }

        if (session.UserId != userId)
        {
            throw new Volo.Abp.BusinessException("Identity.SessionNotBelongToUser")
                .WithData("SessionId", sessionId)
                .WithData("UserId", userId);
        }

        await SessionManager.DeleteAsync(sessionId);
    }

    /// <summary>
    /// 终止当前用户的所有其他会话（保留当前会话）
    /// </summary>
    public virtual async Task RevokeAllOtherSessionsAsync()
    {
        var currentSessionId = CurrentUser.FindClaim(AbpClaimTypes.SessionId)?.Value;
        await SessionManager.DeleteAllAsync(CurrentUser.Id!.Value, exceptSessionId: currentSessionId);
    }

    /// <summary>
    /// 终止指定用户的所有会话（需要管理权限）
    /// </summary>
    [Authorize(IdentityPermissions.Sessions.Revoke)]
    public virtual Task RevokeAllUserSessionsAsync(Guid userId)
    {
        return SessionManager.DeleteAllAsync(userId);
    }
}
