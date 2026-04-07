using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace Censeq.Identity;

/// <summary>
/// 用户会话管理 API
/// </summary>
[RemoteService(Name = IdentityRemoteServiceConsts.RemoteServiceName)]
[Area(IdentityRemoteServiceConsts.ModuleName)]
[ControllerName("Session")]
[Route("api/identity/sessions")]
public class IdentitySessionController : AbpControllerBase, IIdentitySessionAppService
{
    protected IIdentitySessionAppService SessionAppService { get; }

    public IdentitySessionController(IIdentitySessionAppService sessionAppService)
    {
        SessionAppService = sessionAppService;
    }

    /// <summary>
    /// 获取当前用户的所有会话
    /// </summary>
    [HttpGet]
    [Route("my-sessions")]
    public virtual Task<List<IdentitySessionDto>> GetMySessionsAsync()
    {
        return SessionAppService.GetMySessionsAsync();
    }

    /// <summary>
    /// 获取指定用户的所有会话（需要管理权限）
    /// </summary>
    [HttpGet]
    [Route("user/{userId}")]
    public virtual Task<List<IdentitySessionDto>> GetUserSessionsAsync(Guid userId)
    {
        return SessionAppService.GetUserSessionsAsync(userId);
    }

    /// <summary>
    /// 终止当前用户的指定会话
    /// </summary>
    [HttpDelete]
    [Route("my-sessions/{sessionId}")]
    public virtual Task RevokeMySessionAsync(string sessionId)
    {
        return SessionAppService.RevokeMySessionAsync(sessionId);
    }

    /// <summary>
    /// 终止指定用户的指定会话（需要管理权限）
    /// </summary>
    [HttpDelete]
    [Route("user/{userId}/{sessionId}")]
    public virtual Task RevokeUserSessionAsync(Guid userId, string sessionId)
    {
        return SessionAppService.RevokeUserSessionAsync(userId, sessionId);
    }

    /// <summary>
    /// 终止当前用户的所有其他会话
    /// </summary>
    [HttpDelete]
    [Route("my-sessions/others")]
    public virtual Task RevokeAllOtherSessionsAsync()
    {
        return SessionAppService.RevokeAllOtherSessionsAsync();
    }

    /// <summary>
    /// 终止指定用户的所有会话（需要管理权限）
    /// </summary>
    [HttpDelete]
    [Route("user/{userId}/all")]
    public virtual Task RevokeAllUserSessionsAsync(Guid userId)
    {
        return SessionAppService.RevokeAllUserSessionsAsync(userId);
    }
}
