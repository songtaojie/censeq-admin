using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.DistributedLocking;
using Volo.Abp.Threading;

namespace Censeq.OpenIddict.Tokens;

/// <summary>
/// 令牌清理后台工作器。
/// </summary>
public class TokenCleanupBackgroundWorker : AsyncPeriodicBackgroundWorkerBase
{
    /// <summary>
    /// 分布式锁。
    /// </summary>
    protected IAbpDistributedLock DistributedLock { get; }

    /// <summary>
    /// 初始化 TokenCleanupBackgroundWorker 实例。
    /// </summary>
    /// <param name="timer">timer。</param>
    /// <param name="serviceScopeFactory">服务作用域工厂。</param>
    /// <param name="cleanupOptions">清理配置项。</param>
    /// <param name="distributedLock">distributedLock。</param>
    public TokenCleanupBackgroundWorker(
        AbpAsyncTimer timer,
        IServiceScopeFactory serviceScopeFactory,
        IOptionsMonitor<TokenCleanupOptions> cleanupOptions,
        IAbpDistributedLock distributedLock)
        : base(timer, serviceScopeFactory)
    {
        DistributedLock = distributedLock;
        timer.Period = cleanupOptions.CurrentValue.CleanupPeriod;
    }

    /// <summary>
    /// 异步执行后台任务。
    /// </summary>
    /// <param name="workerContext">工作器上下文。</param>
    /// <returns>表示异步操作的任务。</returns>
    protected async override Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
    {
        await using (var handle = await DistributedLock.TryAcquireAsync(nameof(TokenCleanupBackgroundWorker)))
        {
            Logger.LogInformation($"Lock is acquired for {nameof(TokenCleanupBackgroundWorker)}");

            if (handle != null)
            {
                await workerContext
                    .ServiceProvider
                    .GetRequiredService<TokenCleanupService>()
                    .CleanAsync();

                Logger.LogInformation($"Lock is released for {nameof(TokenCleanupBackgroundWorker)}");
                return;
            }

            Logger.LogInformation($"Handle is null because of the locking for : {nameof(TokenCleanupBackgroundWorker)}");
        }
    }
}
