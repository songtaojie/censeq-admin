using Censeq.Admin.MultiTenancy;
using Censeq.AuditLogging;
using Censeq.FeatureManagement;
using Censeq.Identity;
using Censeq.LocalizationManagement;
using Censeq.OpenIddict;
using Censeq.PermissionManagement.Identity;
using Censeq.PermissionManagement.OpenIddict;
using Censeq.TenantManagement;
using MailKit.Security;
using Volo.Abp.Features;
using Volo.Abp.Localization;

namespace Censeq.Admin;

[DependsOn(
    typeof(AbpExceptionHandlingModule),
    typeof(AbpJsonModule),
    typeof(AbpMultiTenancyModule),
    typeof(CenseqAdminDomainSharedModule),
    typeof(CenseqIdentityDomainModule),
    typeof(CenseqOpenIddictDomainModule),
    typeof(CenseqPermissionManagementDomainOpenIddictModule),
    typeof(CenseqPermissionManagementDomainIdentityModule),
    typeof(AbpFeaturesModule),
    typeof(AbpCachingModule),
    typeof(CenseqAuditLoggingDomainModule),
    typeof(CenseqTenantManagementDomainModule),
    typeof(CenseqFeatureManagementDomainModule),
    typeof(CenseqLocalizationManagementDomainModule)
)]
public class CenseqAdminDomainModule : AbpModule
{
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

        //#if DEBUG
        //        context.Services.Replace(ServiceDescriptor.Singleton<IEmailSender, NullEmailSender>());
        //#endif
    }

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        // 在所有模块注册完成后，为每个已注册资源添加数据库翻译贡献者
        // 优先级：租户 DB > Host DB > JSON 文件（JSON contributor 先加载，DB contributor 后加载覆盖）
        Configure<AbpLocalizationOptions>(options =>
        {
            foreach (var resource in options.Resources.Values)
            {
                resource.Contributors.Add(new DbLocalizationResourceContributor());
            }
        });
    }
}
