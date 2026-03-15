using System;
using Volo.Abp.ObjectExtending.Modularity;

namespace Censeq.Identity.ObjectExtending;

public static class IdentityModuleExtensionConfigurationDictionaryExtensions
{
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
