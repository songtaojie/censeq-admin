using System.Collections.Generic;
using Volo.Abp.Collections;

namespace Censeq.PermissionManagement;

public class PermissionManagementOptions
{
    public ITypeList<IPermissionManagementProvider> ManagementProviders { get; }
    public Dictionary<string, string> ProviderPolicies { get; }

    public bool SaveStaticPermissionsToDatabase { get; set; } = true;
    public bool IsDynamicPermissionStoreEnabled { get; set; }

    public PermissionManagementOptions()
    {
        ManagementProviders = new TypeList<IPermissionManagementProvider>();
        ProviderPolicies = [];
    }
}
