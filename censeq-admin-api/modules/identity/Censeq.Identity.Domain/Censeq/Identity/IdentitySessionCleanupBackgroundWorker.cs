using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;

namespace Censeq.Identity;

/// <summary>
/// Session 清理后台任务
/// 定期清理过期的会话
/// </summary>
public class IdentitySessionCleanupBackgroundWorker : AsyncPeriodicBackgroundWorkerBase
{
    public IdentitySessionCleanupBackgroundWorker(
        AbpAsyncTimer timer,
        IServiceScopeFactory serviceScopeFactory,
        IOptions<IdentitySessionCleanupOptions> options)
        : base(timer, serviceScopeFactory)
    {
        Timer.Period = options.Value.CleanupPeriod;
    }

    protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
    {
        Logger.LogDebug("Starting session cleanup background job");

        var sessionManager = workerContext.ServiceProvider.GetRequiredService<IdentitySessionManager>();
        var options = workerContext.ServiceProvider.GetRequiredService<IOptions<IdentitySessionOptions>>().Value;

        if (!options.Enabled)
        {
            Logger.LogDebug("Session management is disabled, skipping cleanup");
            return;
        }

        // 清理超过最大空闲时间的会话
        if (options.MaxInactiveTime.HasValue)
        {
            try
            {
                await sessionManager.DeleteInactiveSessionsAsync(options.MaxInactiveTime.Value);
                Logger.LogInformation("Cleaned up sessions inactive for more than {InactiveTime}", options.MaxInactiveTime.Value);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to clean up inactive sessions");
            }
        }

        // 清理超过最大存活时间的会话
        if (options.MaxSessionLifetime.HasValue)
        {
            try
            {
                await sessionManager.DeleteInactiveSessionsAsync(options.MaxSessionLifetime.Value);
                Logger.LogInformation("Cleaned up sessions older than {Lifetime}", options.MaxSessionLifetime.Value);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to clean up old sessions");
            }
        }

        Logger.LogDebug("Session cleanup background job completed");
    }
}

/// <summary>
/// Session 清理选项
/// </summary>
public class IdentitySessionCleanupOptions
{
    /// <summary>
    /// 清理周期（毫秒）
    /// 默认：1小时（3600000 毫秒）
    /// </summary>
    public int CleanupPeriod { get; set; } = 3600000; // 1 hour
}
