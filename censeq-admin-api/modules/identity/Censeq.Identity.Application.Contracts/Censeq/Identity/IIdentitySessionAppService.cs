using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Censeq.Identity;

/// <summary>
/// 用户会话应用服务接口
/// </summary>
public interface IIdentitySessionAppService : IApplicationService
{
    /// <summary>
    /// 获取当前用户的所有会话
    /// </summary>
    Task<List<IdentitySessionDto>> GetMySessionsAsync();

    /// <summary>
    /// 获取指定用户的所有会话（需要管理权限）
    /// </summary>
    Task<List<IdentitySessionDto>> GetUserSessionsAsync(Guid userId);

    /// <summary>
    /// 终止当前用户的指定会话
    /// </summary>
    Task RevokeMySessionAsync(string sessionId);

    /// <summary>
    /// 终止指定用户的指定会话（需要管理权限）
    /// </summary>
    Task RevokeUserSessionAsync(Guid userId, string sessionId);

    /// <summary>
    /// 终止当前用户的所有其他会话（保留当前会话）
    /// </summary>
    Task RevokeAllOtherSessionsAsync();

    /// <summary>
    /// 终止指定用户的所有会话（需要管理权限）
    /// </summary>
    Task RevokeAllUserSessionsAsync(Guid userId);
}
