using Censeq.AuditLogging.Entities;
using Censeq.AuditLogging.ObjectExtending;
using Volo.Abp.Auditing;
using Volo.Abp.Domain;
using Volo.Abp.ExceptionHandling;
using Volo.Abp.Json;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending.Modularity;
using Volo.Abp.Threading;

namespace Censeq.AuditLogging;

/// <summary>
/// 审计日志领域模块。
/// </summary>
[DependsOn(typeof(AbpAuditingModule))]
[DependsOn(typeof(AbpDddDomainModule))]
[DependsOn(typeof(CenseqAuditLoggingDomainSharedModule))]
[DependsOn(typeof(AbpExceptionHandlingModule))]
[DependsOn(typeof(AbpJsonModule))]
public class CenseqAuditLoggingDomainModule : AbpModule
{
    /// <summary>
    /// 一次性执行器。
    /// </summary>
    private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();

    /// <summary>
    /// 后置配置 CenseqAuditLoggingDomainModule 服务。
    /// </summary>
    /// <param name="context">当前上下文。</param>
    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        OneTimeRunner.Run(() =>
        {
            ModuleExtensionConfigurationHelper.ApplyEntityConfigurationToEntity(
            AuditLoggingModuleExtensionConsts.ModuleName,
            AuditLoggingModuleExtensionConsts.EntityNames.AuditLog,
            typeof(AuditLog)
            );

            ModuleExtensionConfigurationHelper.ApplyEntityConfigurationToEntity(
                AuditLoggingModuleExtensionConsts.ModuleName,
                AuditLoggingModuleExtensionConsts.EntityNames.AuditLogAction,
                typeof(AuditLogAction)
            );

            ModuleExtensionConfigurationHelper.ApplyEntityConfigurationToEntity(
                AuditLoggingModuleExtensionConsts.ModuleName,
                AuditLoggingModuleExtensionConsts.EntityNames.EntityChange,
                typeof(EntityChange)
            );
        });
    }
}
