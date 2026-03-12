using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;

namespace Censeq.PermissionManagement;

public class PermissionFinder : IPermissionFinder, ITransientDependency
{
    protected IPermissionManager PermissionManager { get; }

    public PermissionFinder(IPermissionManager permissionManager)
    {
        PermissionManager = permissionManager;
    }

    public virtual async Task<List<IsGrantedResponse>> IsGrantedAsync(List<IsGrantedRequest> requests)
    {
        var result = new List<IsGrantedResponse>(requests.Count);
        foreach (var item in requests)
        {
            if (item.PermissionNames == null) continue;
            var permissionWithGrantedProviders = await PermissionManager.GetAsync(item.PermissionNames, UserPermissionValueProvider.ProviderName, item.UserId.ToString());
            result.Add(new IsGrantedResponse
            {
                UserId = item.UserId,
                Permissions = permissionWithGrantedProviders.Result.ToDictionary(x => x.Name, x => x.IsGranted)
            });
        }
        return result;
    }
}
