using System.Collections.Generic;
using Volo.Abp;

namespace Censeq.PermissionManagement;

public class MultiplePermissionWithGrantedProviders
{
    public List<PermissionWithGrantedProviders> Result { get; }

    public MultiplePermissionWithGrantedProviders()
    {
        Result = [];
    }

    public MultiplePermissionWithGrantedProviders(string[] names)
    {
        Check.NotNull(names, nameof(names));
        Result = new List<PermissionWithGrantedProviders>(names.Length);
        foreach (var name in names)
            Result.Add(new PermissionWithGrantedProviders(name, false));
    }
}
