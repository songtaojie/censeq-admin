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

[DependsOn(typeof(AbpAuthorizationModule))]
[DependsOn(typeof(AbpDddDomainModule))]
[DependsOn(typeof(CenseqPermissionManagementDomainSharedModule))]
public class CenseqPermissionManagementDomainModule : AbpModule
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private Task? _initializeDynamicPermissionsTask;

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

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        AsyncHelper.RunSync(() => OnApplicationInitializationAsync(context));
    }

    public override Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        InitializeDynamicPermissions(context);
        return Task.CompletedTask;
    }

    public override Task OnApplicationShutdownAsync(ApplicationShutdownContext context)
    {
        _cancellationTokenSource.Cancel();
        return Task.CompletedTask;
    }

    public Task GetInitializeDynamicPermissionsTask()
    {
        return _initializeDynamicPermissionsTask ?? Task.CompletedTask;
    }

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
