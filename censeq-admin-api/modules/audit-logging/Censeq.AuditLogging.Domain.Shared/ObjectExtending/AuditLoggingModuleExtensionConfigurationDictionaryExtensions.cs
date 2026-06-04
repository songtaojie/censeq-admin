using System;
using Volo.Abp.ObjectExtending.Modularity;

namespace Censeq.AuditLogging.ObjectExtending;

/// <summary>
/// 审计日志模块扩展配置字典扩展方法。
/// </summary>
public static class AuditLoggingModuleExtensionConfigurationDictionaryExtensions
{
    /// <summary>
    /// 配置审计日志模块扩展。
    /// </summary>
    /// <param name="modules">modules。</param>
    /// <param name="configureAction">配置操作。</param>
    /// <returns>返回结果。</returns>
    public static ModuleExtensionConfigurationDictionary ConfigureAuditLogging(
        this ModuleExtensionConfigurationDictionary modules,
        Action<AuditLoggingModuleExtensionConfiguration> configureAction)
    {
        return modules.ConfigureModule(
            AuditLoggingModuleExtensionConsts.ModuleName,
            configureAction
        );
    }
}
