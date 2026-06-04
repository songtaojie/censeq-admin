using System;
using Volo.Abp.ObjectExtending.Modularity;

namespace Censeq.AuditLogging.ObjectExtending;

/// <summary>
/// 审计日志模块扩展配置。
/// </summary>
public class AuditLoggingModuleExtensionConfiguration : ModuleExtensionConfiguration
{
    /// <summary>
    /// 配置审计日志实体模型。
    /// </summary>
    /// <param name="configureAction">配置操作。</param>
    /// <returns>返回结果。</returns>
    public AuditLoggingModuleExtensionConfiguration ConfigureAuditLog(
        Action<EntityExtensionConfiguration> configureAction)
    {
        return this.ConfigureEntity(
            AuditLoggingModuleExtensionConsts.EntityNames.AuditLog,
            configureAction
        );
    }

    /// <summary>
    /// 配置审计日志操作实体模型。
    /// </summary>
    /// <param name="configureAction">配置操作。</param>
    /// <returns>返回结果。</returns>
    public AuditLoggingModuleExtensionConfiguration ConfigureAuditLogAction(
        Action<EntityExtensionConfiguration> configureAction)
    {
        return this.ConfigureEntity(
            AuditLoggingModuleExtensionConsts.EntityNames.AuditLogAction,
            configureAction
        );
    }

    /// <summary>
    /// 配置实体变更模型。
    /// </summary>
    /// <param name="configureAction">配置操作。</param>
    /// <returns>返回结果。</returns>
    public AuditLoggingModuleExtensionConfiguration ConfigureEntityChange(
        Action<EntityExtensionConfiguration> configureAction)
    {
        return this.ConfigureEntity(
            AuditLoggingModuleExtensionConsts.EntityNames.EntityChange,
            configureAction
        );
    }
}
