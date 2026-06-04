using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace Censeq.PermissionManagement;

/// <summary>
/// 权限管理领域模块。
/// </summary>
[DependsOn(typeof(AbpAuthorizationModule))]
[DependsOn(typeof(AbpDddDomainModule))]
[DependsOn(typeof(CenseqPermissionManagementDomainSharedModule))]
public class CenseqPermissionManagementDomainModule : AbpModule
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private Task? _initializeDynamicPermissionsTask;

    /// <summary>
    /// 配置权限管理模块服务。
    /// </summary>
    /// <param name="context">服务配置上下文。</param>
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        if (context.Services.IsDataMigrationEnvironment())
        {
            Configure<PermissionManagementOptions>(options =>
            {
                options.SaveStaticPermissionsToDatabase = false;
                options.IsDynamicPermissionStoreEnabled = false;
            });
        }
    }

    /// <summary>
    /// 应用初始化时启动动态权限初始化流程。
    /// </summary>
    /// <param name="context">应用初始化上下文。</param>
    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        AsyncHelper.RunSync(() => OnApplicationInitializationAsync(context));
    }

    /// <summary>
    /// 应用初始化时异步等待动态权限初始化流程完成。
    /// </summary>
    /// <param name="context">应用初始化上下文。</param>
    /// <returns>表示异步操作的任务。</returns>
    public override Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        InitializeDynamicPermissions(context);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 应用关闭时等待动态权限初始化流程完成。
    /// </summary>
    /// <param name="context">应用关闭上下文。</param>
    /// <returns>表示异步操作的任务。</returns>
    public override Task OnApplicationShutdownAsync(ApplicationShutdownContext context)
    {
        _cancellationTokenSource.Cancel();
        return Task.CompletedTask;
    }

    /// <summary>
    /// 获取动态权限初始化任务。
    /// </summary>
    /// <returns>动态权限初始化任务。</returns>
    public Task GetInitializeDynamicPermissionsTask()
    {
        return _initializeDynamicPermissionsTask ?? Task.CompletedTask;
    }

    /// <summary>
    /// 初始化动态权限定义。
    /// </summary>
    /// <param name="context">服务配置上下文。</param>
    private void InitializeDynamicPermissions(ApplicationInitializationContext context)
    {
        var options = context.ServiceProvider.GetRequiredService<IOptions<PermissionManagementOptions>>().Value;
        if (!options.SaveStaticPermissionsToDatabase && !options.IsDynamicPermissionStoreEnabled)
            return;

        var rootServiceProvider = context.ServiceProvider.GetRequiredService<IRootServiceProvider>();
        _initializeDynamicPermissionsTask = Task.Run(async () =>
        {
            using var scope = rootServiceProvider.CreateScope();
            var applicationLifetime = scope.ServiceProvider.GetService<IHostApplicationLifetime>();
            var cancellationTokenProvider = scope.ServiceProvider.GetRequiredService<ICancellationTokenProvider>();
            var cancellationToken = applicationLifetime?.ApplicationStopping ?? _cancellationTokenSource.Token;
            try
            {
                using (cancellationTokenProvider.Use(cancellationToken))
                {
                    if (cancellationTokenProvider.Token.IsCancellationRequested) return;
                    await SaveStaticPermissionsToDatabaseAsync(options, scope, cancellationTokenProvider);
                    if (cancellationTokenProvider.Token.IsCancellationRequested) return;
                    await PreCacheDynamicPermissionsAsync(options, scope);
                }
            }
            catch { }
        });
    }

    /// <summary>
    /// 将静态权限定义保存到数据库。
    /// </summary>
    /// <param name="options">权限管理配置。</param>
    /// <param name="scope">服务作用域。</param>
    /// <param name="cancellationTokenProvider">取消令牌提供者。</param>
    /// <returns>表示异步操作的任务。</returns>
    private static async Task SaveStaticPermissionsToDatabaseAsync(PermissionManagementOptions options, IServiceScope scope, ICancellationTokenProvider cancellationTokenProvider)
    {
        if (!options.SaveStaticPermissionsToDatabase) return;
        await Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(8, retryAttempt => TimeSpan.FromSeconds(RandomHelper.GetRandom((int)Math.Pow(2, retryAttempt) * 8, (int)Math.Pow(2, retryAttempt) * 12)))
            .ExecuteAsync(async _ =>
            {
                try
                {
                    await scope.ServiceProvider.GetRequiredService<IStaticPermissionSaver>().SaveAsync();
                }
                catch (Exception ex)
                {
                    scope.ServiceProvider.GetService<ILogger<CenseqPermissionManagementDomainModule>>()?.LogException(ex);
                    throw;
                }
            }, cancellationTokenProvider.Token);
    }

    /// <summary>
    /// 预热动态权限定义缓存。
    /// </summary>
    /// <param name="options">权限管理配置。</param>
    /// <param name="scope">服务作用域。</param>
    /// <returns>表示异步操作的任务。</returns>
    private static async Task PreCacheDynamicPermissionsAsync(PermissionManagementOptions options, IServiceScope scope)
    {
        if (!options.IsDynamicPermissionStoreEnabled) return;
        try
        {
            await scope.ServiceProvider.GetRequiredService<IDynamicPermissionDefinitionStore>().GetGroupsAsync();
        }
        catch (Exception ex)
        {
            scope.ServiceProvider.GetService<ILogger<CenseqPermissionManagementDomainModule>>()?.LogException(ex);
            throw;
        }
    }
}
