using System;
using Volo.Abp.ObjectExtending.Modularity;

namespace Volo.Abp.ObjectExtending;

/// <summary>
/// OpenIddict 模块扩展配置字典扩展方法。
/// </summary>
public static class OpenIddictModuleExtensionConfigurationDictionaryExtensions
{
    /// <summary>
    /// 配置 OpenIddict 数据库模型。
    /// </summary>
    /// <param name="modules">modules。</param>
    /// <param name="configureAction">configure操作。</param>
    /// <returns>操作结果。</returns>
    public static ModuleExtensionConfigurationDictionary ConfigureOpenIddict(
        this ModuleExtensionConfigurationDictionary modules,
        Action<OpenIddictModuleExtensionConfiguration> configureAction)
    {
        return modules.ConfigureModule(
            OpenIddictModuleExtensionConsts.ModuleName,
            configureAction
        );
    }
}
