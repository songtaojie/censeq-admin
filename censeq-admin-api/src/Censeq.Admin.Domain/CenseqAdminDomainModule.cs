using Censeq.Abp.TenantManagement;
using Censeq.Admin.Entities;
using Censeq.Admin.FeatureManagement;
using Censeq.Admin.MultiTenancy;
using Censeq.AuditLogging;
using Censeq.SettingManagement;
using MailKit.Security;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.Features;

namespace Censeq.Admin;

[DependsOn(
    typeof(CenseqAuditLoggingDomainModule),
    typeof(CenseqSettingManagementDomainModule),
    typeof(AbpExceptionHandlingModule),
    typeof(AbpJsonModule),
    typeof(AbpMultiTenancyModule),
    typeof(CenseqAdminDomainSharedModule),
    typeof(AbpBackgroundJobsDomainModule),
    typeof(AbpIdentityDomainModule),
    typeof(AbpOpenIddictDomainModule),
    typeof(AbpPermissionManagementDomainOpenIddictModule),
    typeof(AbpPermissionManagementDomainIdentityModule),
    typeof(AbpMailKitModule),
    typeof(AbpFeaturesModule),
    typeof(AbpCachingModule)
)]
public class CenseqAdminDomainModule : AbpModule
{
    private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Languages.Add(new LanguageInfo("ar", "ar", "العربية"));
            options.Languages.Add(new LanguageInfo("cs", "cs", "Čeština"));
            options.Languages.Add(new LanguageInfo("en", "en", "English"));
            options.Languages.Add(new LanguageInfo("en-GB", "en-GB", "English (UK)"));
            options.Languages.Add(new LanguageInfo("hu", "hu", "Magyar"));
            options.Languages.Add(new LanguageInfo("hr", "hr", "Croatian"));
            options.Languages.Add(new LanguageInfo("fi", "fi", "Finnish"));
            options.Languages.Add(new LanguageInfo("fr", "fr", "Français"));
            options.Languages.Add(new LanguageInfo("hi", "hi", "Hindi"));
            options.Languages.Add(new LanguageInfo("it", "it", "Italiano"));
            options.Languages.Add(new LanguageInfo("pt-BR", "pt-BR", "Português"));
            options.Languages.Add(new LanguageInfo("ru", "ru", "Русский"));
            options.Languages.Add(new LanguageInfo("sk", "sk", "Slovak"));
            options.Languages.Add(new LanguageInfo("tr", "tr", "Türkçe"));
            options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文"));
            options.Languages.Add(new LanguageInfo("zh-Hant", "zh-Hant", "繁體中文"));
            options.Languages.Add(new LanguageInfo("de-DE", "de-DE", "Deutsch"));
            options.Languages.Add(new LanguageInfo("es", "es", "Español"));
        });

        Configure<AbpMultiTenancyOptions>(options =>
        {
            options.IsEnabled = MultiTenancyConsts.IsEnabled;
        });
        Configure<AbpMailKitOptions>(options =>
        {
            options.SecureSocketOption = SecureSocketOptions.SslOnConnect;
        });

        Configure<AbpDistributedEntityEventOptions>(options =>
        {
            options.EtoMappings.Add<Tenant, TenantEto>();
        });

        Configure<FeatureManagementOptions>(options =>
        {
            options.Providers.Add<DefaultValueFeatureManagementProvider>();
            options.Providers.Add<EditionFeatureManagementProvider>();

            //TODO: Should be moved to the Tenant Management module
            options.Providers.Add<TenantFeatureManagementProvider>();
            options.ProviderPolicies[TenantFeatureValueProvider.ProviderName] = "AbpTenantManagement.Tenants.ManageFeatures";
        });

        if (context.Services.IsDataMigrationEnvironment())
        {
            Configure<FeatureManagementOptions>(options =>
            {
                options.SaveStaticFeaturesToDatabase = false;
                options.IsDynamicFeatureStoreEnabled = false;
            });
        }

        //#if DEBUG
        //        context.Services.Replace(ServiceDescriptor.Singleton<IEmailSender, NullEmailSender>());
        //#endif
    }

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        OneTimeRunner.Run(() =>
        {
            ModuleExtensionConfigurationHelper.ApplyEntityConfigurationToEntity(
                TenantManagementModuleExtensionConsts.ModuleName,
                TenantManagementModuleExtensionConsts.EntityNames.Tenant,
                typeof(Tenant)
            );
        });
    }

    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private Task _initializeDynamicFeaturesTask;

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        AsyncHelper.RunSync(() => OnApplicationInitializationAsync(context));
    }

    public override Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        InitializeDynamicFeatures(context);
        return Task.CompletedTask;
    }

    public override Task OnApplicationShutdownAsync(ApplicationShutdownContext context)
    {
        _cancellationTokenSource.Cancel();
        return Task.CompletedTask;
    }

    public Task GetInitializeDynamicFeaturesTask()
    {
        return _initializeDynamicFeaturesTask ?? Task.CompletedTask;
    }

    private void InitializeDynamicFeatures(ApplicationInitializationContext context)
    {
        var options = context
            .ServiceProvider
            .GetRequiredService<IOptions<FeatureManagementOptions>>()
            .Value;

        if (!options.SaveStaticFeaturesToDatabase && !options.IsDynamicFeatureStoreEnabled)
        {
            return;
        }

        var rootServiceProvider = context.ServiceProvider.GetRequiredService<IRootServiceProvider>();

        _initializeDynamicFeaturesTask = Task.Run(async () =>
        {
            using var scope = rootServiceProvider.CreateScope();
            var applicationLifetime = scope.ServiceProvider.GetService<IHostApplicationLifetime>();
            var cancellationTokenProvider = scope.ServiceProvider.GetRequiredService<ICancellationTokenProvider>();
            var cancellationToken = applicationLifetime?.ApplicationStopping ?? _cancellationTokenSource.Token;

            try
            {
                using (cancellationTokenProvider.Use(cancellationToken))
                {
                    if (cancellationTokenProvider.Token.IsCancellationRequested)
                    {
                        return;
                    }

                    await SaveStaticFeaturesToDatabaseAsync(options, scope, cancellationTokenProvider);

                    if (cancellationTokenProvider.Token.IsCancellationRequested)
                    {
                        return;
                    }

                    await PreCacheDynamicFeaturesAsync(options, scope);
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause (No need to log since it is logged above)
            catch { }
        });
    }

    private static async Task SaveStaticFeaturesToDatabaseAsync(
        FeatureManagementOptions options,
        IServiceScope scope,
        ICancellationTokenProvider cancellationTokenProvider)
    {
        if (!options.SaveStaticFeaturesToDatabase)
        {
            return;
        }

        await Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                8,
                retryAttempt => TimeSpan.FromSeconds(
                    RandomHelper.GetRandom(
                        (int)Math.Pow(2, retryAttempt) * 8,
                        (int)Math.Pow(2, retryAttempt) * 12)
                )
            )
            .ExecuteAsync(async _ =>
            {
                try
                {
                    // ReSharper disable once AccessToDisposedClosure
                    await scope
                        .ServiceProvider
                        .GetRequiredService<IStaticFeatureSaver>()
                        .SaveAsync();
                }
                catch (Exception ex)
                {
                    // ReSharper disable once AccessToDisposedClosure
                    scope.ServiceProvider
                        .GetService<ILogger<CenseqAdminDomainModule>>()?
                        .LogException(ex);

                    throw; // Polly will catch it
                }
            }, cancellationTokenProvider.Token);
    }

    private static async Task PreCacheDynamicFeaturesAsync(FeatureManagementOptions options, IServiceScope scope)
    {
        if (!options.IsDynamicFeatureStoreEnabled)
        {
            return;
        }

        try
        {
            // Pre-cache features, so first request doesn't wait
            await scope
                .ServiceProvider
                .GetRequiredService<IDynamicFeatureDefinitionStore>()
                .GetGroupsAsync();
        }
        catch (Exception ex)
        {
            // ReSharper disable once AccessToDisposedClosure
            scope
                .ServiceProvider
                .GetService<ILogger<CenseqAdminDomainModule>>()?
                .LogException(ex);

            throw; // It will be cached in InitializeDynamicFeatures
        }
    }
}
