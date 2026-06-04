using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Censeq.Account.Localization;
using Censeq.Identity;

namespace Censeq.Account;

/// <summary>
/// 账户 HTTP API 模块。
/// </summary>
[DependsOn(
    typeof(CenseqAccountApplicationContractsModule),
    typeof(CenseqIdentityHttpApiModule),
    typeof(AbpAspNetCoreMvcModule))]
public class CenseqAccountHttpApiModule : AbpModule
{
    /// <summary>
    /// 预配置 账户 HTTP API 模块服务。
    /// </summary>
    /// <param name="context">当前上下文。</param>
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(CenseqAccountHttpApiModule).Assembly);
        });
    }

    /// <summary>
    /// 配置 账户 HTTP API 模块服务。
    /// </summary>
    /// <param name="context">当前上下文。</param>
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<AccountResource>()
                .AddBaseTypes(typeof(AbpUiResource));
        });
    }
}
