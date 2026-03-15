using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Caching;
using Volo.Abp.DistributedLocking;
using Volo.Abp.Domain;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.ObjectExtending.Modularity;
using Censeq.OpenIddict.Applications;
using Censeq.OpenIddict.Authorizations;
using Censeq.OpenIddict.Scopes;
using Censeq.OpenIddict.Tokens;
using Volo.Abp.Threading;

namespace Censeq.OpenIddict;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(CenseqIdentityDomainModule),
    typeof(CenseqOpenIddictDomainSharedModule),
    typeof(AbpDistributedLockingAbstractionsModule),
    typeof(AbpCachingModule),
    typeof(AbpGuidsModule)
)]
public class CenseqOpenIddictDomainModule : AbpModule
{
    private readonly static OneTimeRunner OneTimeRunner = new OneTimeRunner();

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        AddOpenIddictCore(context.Services);
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        AsyncHelper.RunSync(() => OnApplicationInitializationAsync(context));
    }

    public async override Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        var options = context.ServiceProvider.GetRequiredService<IOptions<TokenCleanupOptions>>().Value;
        if (options.IsCleanupEnabled)
        {
            await context.ServiceProvider
                .GetRequiredService<IBackgroundWorkerManager>()
                .AddAsync(context.ServiceProvider.GetRequiredService<TokenCleanupBackgroundWorker>());
        }
    }

    private void AddOpenIddictCore(IServiceCollection services)
    {
        var openIddictBuilder = services.AddOpenIddict()
            .AddCore(builder =>
            {
                builder
                    .SetDefaultApplicationEntity<OpenIddictApplicationModel>()
                    .SetDefaultAuthorizationEntity<OpenIddictAuthorizationModel>()
                    .SetDefaultScopeEntity<OpenIddictScopeModel>()
                    .SetDefaultTokenEntity<OpenIddictTokenModel>();

                builder
                    .AddApplicationStore<CenseqOpenIddictApplicationStore>()
                    .AddAuthorizationStore<CenseqOpenIddictAuthorizationStore>()
                    .AddScopeStore<CenseqOpenIddictScopeStore>()
                    .AddTokenStore<CenseqOpenIddictTokenStore>();

                builder.ReplaceApplicationManager(typeof(CenseqApplicationManager));
                builder.ReplaceAuthorizationManager(typeof(CenseqAuthorizationManager));
                builder.ReplaceScopeManager(typeof(CenseqScopeManager));
                builder.ReplaceTokenManager(typeof(CenseqTokenManager));

                builder.Services.TryAddScoped(provider => (ICenseqApplicationManager)provider.GetRequiredService<IOpenIddictApplicationManager>());

                services.ExecutePreConfiguredActions(builder);
            });

        services.ExecutePreConfiguredActions(openIddictBuilder);
    }

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        OneTimeRunner.Run(() =>
        {
            ModuleExtensionConfigurationHelper.ApplyEntityConfigurationToEntity(
                OpenIddictModuleExtensionConsts.ModuleName,
                OpenIddictModuleExtensionConsts.EntityNames.Application,
                typeof(OpenIddictApplication)
            );

            ModuleExtensionConfigurationHelper.ApplyEntityConfigurationToEntity(
                OpenIddictModuleExtensionConsts.ModuleName,
                OpenIddictModuleExtensionConsts.EntityNames.Application,
                typeof(OpenIddictApplicationModel)
            );

            ModuleExtensionConfigurationHelper.ApplyEntityConfigurationToEntity(
                OpenIddictModuleExtensionConsts.ModuleName,
                OpenIddictModuleExtensionConsts.EntityNames.Authorization,
                typeof(OpenIddictAuthorization)
            );

            ModuleExtensionConfigurationHelper.ApplyEntityConfigurationToEntity(
                OpenIddictModuleExtensionConsts.ModuleName,
                OpenIddictModuleExtensionConsts.EntityNames.Authorization,
                typeof(OpenIddictAuthorizationModel)
            );

            ModuleExtensionConfigurationHelper.ApplyEntityConfigurationToEntity(
                OpenIddictModuleExtensionConsts.ModuleName,
                OpenIddictModuleExtensionConsts.EntityNames.Scope,
                typeof(OpenIddictScope)
            );

            ModuleExtensionConfigurationHelper.ApplyEntityConfigurationToEntity(
                OpenIddictModuleExtensionConsts.ModuleName,
                OpenIddictModuleExtensionConsts.EntityNames.Scope,
                typeof(OpenIddictScopeModel)
            );

            ModuleExtensionConfigurationHelper.ApplyEntityConfigurationToEntity(
                OpenIddictModuleExtensionConsts.ModuleName,
                OpenIddictModuleExtensionConsts.EntityNames.Token,
                typeof(OpenIddictToken)
            );

            ModuleExtensionConfigurationHelper.ApplyEntityConfigurationToEntity(
                OpenIddictModuleExtensionConsts.ModuleName,
                OpenIddictModuleExtensionConsts.EntityNames.Token,
                typeof(OpenIddictTokenModel)
            );
        });
    }
}
