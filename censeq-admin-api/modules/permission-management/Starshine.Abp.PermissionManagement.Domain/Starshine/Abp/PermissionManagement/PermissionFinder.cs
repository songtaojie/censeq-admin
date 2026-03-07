using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;

namespace Censeq.Abp.PermissionManagement;

/// <summary>
/// Ȩ�޲�����
/// </summary>
public class PermissionFinder : IPermissionFinder, ITransientDependency
{
    /// <summary>
    /// Ȩ�޹�����
    /// </summary>
    protected IPermissionManager PermissionManager { get; }

    /// <summary>
    /// Ȩ�޲�����
    /// </summary>
    /// <param name="permissionManager">Ȩ�޹�����</param>
    public PermissionFinder(IPermissionManager permissionManager)
    {
        PermissionManager = permissionManager;
    }

    /// <summary>
    /// �Ƿ���Ȩ
    /// </summary>
    /// <param name="requests">�Ƿ���������</param>
    /// <returns></returns>
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
