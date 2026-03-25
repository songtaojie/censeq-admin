using System;
using Volo.Abp.ObjectExtending.Modularity;

namespace Censeq.TenantManagement.ObjectExtending;

public class TenantManagementModuleExtensionConfiguration : ModuleExtensionConfiguration
{
    public TenantManagementModuleExtensionConfiguration ConfigureTenant(
        Action<EntityExtensionConfiguration> configureAction)
    {
        return this.ConfigureEntity(
            TenantManagementModuleExtensionConsts.EntityNames.Tenant,
            configureAction
        );
    }
}
