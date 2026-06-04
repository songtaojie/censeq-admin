using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;

namespace Censeq.PermissionManagement;

/// <summary>
/// 权限授予状态查询器。
/// </summary>
public class PermissionFinder : IPermissionFinder, ITransientDependency
{
    /// <summary>
    /// 权限管理器。
    /// </summary>
    protected IPermissionManager PermissionManager { get; }

    /// <summary>
    /// 初始化 PermissionFinder 实例。
    /// </summary>
    /// <param name="permissionManager">权限管理器。</param>
    public PermissionFinder(IPermissionManager permissionManager)
    {
        PermissionManager = permissionManager;
    }

    /// <summary>
    /// 批量检查用户权限授予状态。
    /// </summary>
    /// <param name="requests">权限授予状态查询请求列表。</param>
    /// <returns>按用户返回的权限授予状态列表。</returns>
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
