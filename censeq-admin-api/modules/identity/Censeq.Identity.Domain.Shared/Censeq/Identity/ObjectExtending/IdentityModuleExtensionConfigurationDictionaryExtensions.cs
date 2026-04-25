using System;
using Volo.Abp.ObjectExtending.Modularity;

namespace Censeq.Identity.ObjectExtending;

/// <summary>
/// 身份模块扩展配置字典扩展方法
/// </summary>
public static class IdentityModuleExtensionConfigurationDictionaryExtensions
{
    /// <summary>
    /// 配置身份模块扩展
    /// </summary>
    /// <param name="modules">模块扩展配置字典</param>
    /// <param name="configureAction">配置操作</param>
    /// <returns>模块扩展配置字典</returns>
    /// <summary>
    /// 模块Extension配置Dictionary
    /// </summary>
    public static ModuleExtensionConfigurationDictionary ConfigureIdentity(
        this ModuleExtensionConfigurationDictionary modules,
        Action<IdentityModuleExtensionConfiguration> configureAction)
    {
        return modules.ConfigureModule(
            IdentityModuleExtensionConsts.ModuleName,
            configureAction
        );
    }
}
