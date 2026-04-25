using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Censeq.Identity.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Timing;

namespace Censeq.Identity;

/// <summary>
/// 身份会话管理器
/// </summary>
public class IdentitySessionManager : DomainService
{
    /// <summary>
    /// I身份会话仓储
    /// </summary>
    protected IIdentitySessionRepository SessionRepository { get; }
    /// <summary>
    /// IOptions<Identity会话Options>
    /// </summary>
    protected IOptions<IdentitySessionOptions> Options { get; }

    public IdentitySessionManager(
        IIdentitySessionRepository sessionRepository,
        IOptions<IdentitySessionOptions> options)
    {
        SessionRepository = sessionRepository;
        Options = options;
    }

    /// <summary>
    /// 创建新的会话
    /// </summary>
    public virtual async Task<IdentitySession> CreateAsync(
        Guid userId,
        string device = IdentitySessionDevices.Web,
        string deviceInfo = "",
        string clientId = "",
        string ipAddresses = "",
        CancellationToken cancellationToken = default)
    {
        var sessionId = GuidGenerator.Create().ToString("N");

        var session = new IdentitySession(
            GuidGenerator.Create(),
            sessionId,
            device,
            deviceInfo,
            userId,
            CurrentTenant.Id,
            clientId,
            ipAddresses,
            Clock.Now,
            Clock.Now
        );

        await SessionRepository.InsertAsync(session, cancellationToken: cancellationToken);

        Logger.LogDebug("Created new session for user {UserId}: {SessionId}", userId, sessionId);

        return session;
    }

    /// <summary>
    /// 根据 SessionId 获取会话
    /// </summary>
    public virtual async Task<IdentitySession> GetAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        var session = await SessionRepository.FindAsync(sessionId, cancellationToken);
        if (session == null)
        {
            throw new BusinessException("Identity.SessionNotFound", $"Session {sessionId} not found");
        }
        return session;
    }

    /// <summary>
    /// 根据 SessionId 查找会话（可能返回 null）
    /// </summary>
    public virtual Task<IdentitySession?> FindAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        return SessionRepository.FindAsync(sessionId, cancellationToken)!;
    }

    /// <summary>
    /// 更新会话最后访问时间
    /// </summary>
    public virtual async Task UpdateLastAccessedTimeAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        var session = await SessionRepository.FindAsync(sessionId, cancellationToken);
        if (session != null)
        {
            session.UpdateLastAccessedTime(Clock.Now);
            await SessionRepository.UpdateAsync(session, cancellationToken: cancellationToken);
        }
    }

    /// <summary>
    /// 删除指定会话
    /// </summary>
    public virtual async Task DeleteAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        var session = await SessionRepository.FindAsync(sessionId, cancellationToken);
        if (session != null)
        {
            await SessionRepository.DeleteAsync(session, cancellationToken: cancellationToken);
        }
    }

    /// <summary>
    /// 删除用户的所有会话（可排除指定会话）
    /// </summary>
    public virtual Task DeleteAllAsync(Guid userId, string? exceptSessionId = null, CancellationToken cancellationToken = default)
    {
        if (exceptSessionId.IsNullOrWhiteSpace())
        {
            return SessionRepository.DeleteAllAsync(userId, cancellationToken: cancellationToken);
        }

        // 需要先找到要排除的会话的 Id
        return DeleteAllExceptAsync(userId, exceptSessionId, cancellationToken);
    }

    /// <summary>
    /// 删除用户在指定设备上的所有会话
    /// </summary>
    public virtual Task DeleteAllAsync(Guid userId, string device, string? exceptSessionId = null, CancellationToken cancellationToken = default)
    {
        if (exceptSessionId.IsNullOrWhiteSpace())
        {
            return SessionRepository.DeleteAllAsync(userId, device, cancellationToken: cancellationToken);
        }

        return DeleteAllExceptAsync(userId, device, exceptSessionId, cancellationToken);
    }

    /// <summary>
    /// 清理过期的会话
    /// </summary>
    public virtual Task DeleteInactiveSessionsAsync(TimeSpan inactiveTimeSpan, CancellationToken cancellationToken = default)
    {
        return SessionRepository.DeleteAllAsync(inactiveTimeSpan, cancellationToken);
    }

    /// <summary>
    /// 获取用户的会话列表
    /// </summary>
    public virtual Task<List<IdentitySession>> GetListAsync(
        Guid? userId = null,
        string? device = null,
        string? clientId = null,
        CancellationToken cancellationToken = default)
    {
        return SessionRepository.GetListAsync(
            sorting: $"{nameof(IdentitySession.LastAccessed)} desc",
            userId: userId,
            device: device,
            clientId: clientId,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// 获取用户当前活动的会话数
    /// </summary>
    public virtual Task<long> GetCountAsync(Guid? userId = null, CancellationToken cancellationToken = default)
    {
        return SessionRepository.GetCountAsync(userId, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// 检查会话是否有效（存在且未过期）
    /// </summary>
    public virtual async Task<bool> IsSessionValidAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        var session = await SessionRepository.FindAsync(sessionId, cancellationToken);
        if (session == null)
        {
            return false;
        }

        // 检查是否超过最大空闲时间
        if (Options.Value.MaxInactiveTime.HasValue)
        {
            var inactiveTime = Clock.Now - (session.LastAccessed ?? session.SignedIn);
            if (inactiveTime > Options.Value.MaxInactiveTime.Value)
            {
                return false;
            }
        }

        // 检查是否超过最大存活时间
        if (Options.Value.MaxSessionLifetime.HasValue)
        {
            var lifetime = Clock.Now - session.SignedIn;
            if (lifetime > Options.Value.MaxSessionLifetime.Value)
            {
                return false;
            }
        }

        return true;
    }

    #region Private Methods

    private async Task DeleteAllExceptAsync(Guid userId, string exceptSessionId, CancellationToken cancellationToken)
    {
        var exceptSession = await SessionRepository.FindAsync(exceptSessionId, cancellationToken);
        var exceptId = exceptSession?.Id;
        await SessionRepository.DeleteAllAsync(userId, exceptId, cancellationToken);
    }

    private async Task DeleteAllExceptAsync(Guid userId, string device, string exceptSessionId, CancellationToken cancellationToken)
    {
        var exceptSession = await SessionRepository.FindAsync(exceptSessionId, cancellationToken);
        var exceptId = exceptSession?.Id;
        await SessionRepository.DeleteAllAsync(userId, device, exceptId, cancellationToken);
    }

    #endregion
}
