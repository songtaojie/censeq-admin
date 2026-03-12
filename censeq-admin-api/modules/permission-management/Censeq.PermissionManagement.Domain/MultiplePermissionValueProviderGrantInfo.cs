using System.Collections.Generic;
using Volo.Abp;

namespace Censeq.PermissionManagement;

public class MultiplePermissionValueProviderGrantInfo
{
    public Dictionary<string, PermissionValueProviderGrantInfo> Result { get; }

    public MultiplePermissionValueProviderGrantInfo()
    {
        Result = [];
    }

    public MultiplePermissionValueProviderGrantInfo(string[] names)
    {
        Check.NotNull(names, nameof(names));
        Result = new Dictionary<string, PermissionValueProviderGrantInfo>(names.Length);
        foreach (var name in names)
            Result.Add(name, PermissionValueProviderGrantInfo.NonGranted);
    }
}
