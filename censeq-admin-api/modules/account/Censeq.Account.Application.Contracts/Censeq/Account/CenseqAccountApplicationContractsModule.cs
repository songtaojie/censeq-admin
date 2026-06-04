using Censeq.Account.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending.Modularity;
using Volo.Abp.Threading;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;
using Censeq.Identity;
using Censeq.Identity.ObjectExtending;

namespace Censeq.Account;

/// <summary>
/// 账户应用程序契约模块。
/// </summary>
[DependsOn(
    typeof(CenseqIdentityApplicationContractsModule)
)]
public class CenseqAccountApplicationContractsModule : AbpModule
{
    /// <summary>
    /// 一次性执行器。
    /// </summary>
    private static readonly OneTimeRunner OneTimeRunner = new();

    /// <summary>
    /// 配置 账户 应用程序 Contracts 模块服务。
    /// </summary>
    /// <param name="context">当前上下文。</param>
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<CenseqAccountApplicationContractsModule>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<AccountResource>("en")
                .AddBaseTypes(typeof(AbpValidationResource))
                .AddVirtualJson("/Censeq/Account/Localization/Resources");
        });

        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("Censeq.Account", typeof(AccountResource));
        });
    }

    /// <summary>
    /// 后置配置 账户 应用程序 Contracts 模块服务。
    /// </summary>
    /// <param name="context">当前上下文。</param>
    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        OneTimeRunner.Run(() =>
        {
            ModuleExtensionConfigurationHelper.ApplyEntityConfigurationToApi(
                IdentityModuleExtensionConsts.ModuleName,
                IdentityModuleExtensionConsts.EntityNames.User,
                getApiTypes: new[] { typeof(ProfileDto) },
                updateApiTypes: new[] { typeof(UpdateProfileDto) }
            );
        });
    }
}
