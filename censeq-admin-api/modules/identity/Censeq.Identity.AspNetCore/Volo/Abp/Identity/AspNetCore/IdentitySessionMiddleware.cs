using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.Security.Claims;

namespace Censeq.Identity.AspNetCore;

/// <summary>
/// Session 中间件，用于更新会话的最后访问时间
/// </summary>
public class IdentitySessionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<IdentitySessionMiddleware> _logger;

    public IdentitySessionMiddleware(
        RequestDelegate next,
        ILogger<IdentitySessionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await UpdateSessionLastAccessedTimeAsync(context);
        await _next(context);
    }

    protected virtual async Task UpdateSessionLastAccessedTimeAsync(HttpContext context)
    {
        try
        {
            // 只处理已认证的请求
            if (context.User?.Identity?.IsAuthenticated != true)
            {
                return;
            }

            // 获取 SessionId
            var sessionId = context.User.FindFirst(AbpClaimTypes.SessionId)?.Value;
            if (string.IsNullOrEmpty(sessionId))
            {
                return;
            }

            // 检查是否需要更新（避免过于频繁的更新）
            var lastUpdateKey = $"SessionLastUpdate_{sessionId}";
            var lastUpdate = context.Items[lastUpdateKey] as DateTime?;
            if (lastUpdate.HasValue && (DateTime.UtcNow - lastUpdate.Value).TotalMinutes < 1)
            {
                // 距离上次更新不到 1 分钟，跳过
                return;
            }

            // 获取 SessionManager
            var sessionManager = context.RequestServices.GetService<IdentitySessionManager>();
            if (sessionManager == null)
            {
                return;
            }

            // 检查会话是否有效
            var options = context.RequestServices.GetRequiredService<IOptions<IdentitySessionOptions>>().Value;
            if (options.Enabled && !await sessionManager.IsSessionValidAsync(sessionId))
            {
                // 会话已过期，可以在这里处理强制登出逻辑
                _logger.LogWarning("Session {SessionId} has expired", sessionId);
                return;
            }

            // 更新最后访问时间
            await sessionManager.UpdateLastAccessedTimeAsync(sessionId);

            // 记录更新时间
            context.Items[lastUpdateKey] = DateTime.UtcNow;

            _logger.LogDebug("Updated last accessed time for session {SessionId}", sessionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update session last accessed time");
        }
    }
}

/// <summary>
/// Session 中间件扩展方法
/// </summary>
public static class IdentitySessionMiddlewareExtensions
{
    /// <summary>
    /// 使用 Session 中间件
    /// </summary>
    public static IApplicationBuilder UseIdentitySession(this IApplicationBuilder app)
    {
        return app.UseMiddleware<IdentitySessionMiddleware>();
    }
}
