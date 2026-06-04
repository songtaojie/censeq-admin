using System.Collections.Generic;
using System.Threading.Tasks;

namespace Censeq.PermissionManagement;

/// <summary>
/// 权限授予状态查询器接口。
/// </summary>
public interface IPermissionFinder
{
    /// <summary>
    /// 批量检查用户权限授予状态。
    /// </summary>
    /// <param name="requests">权限授予状态查询请求列表。</param>
    /// <returns>按用户返回的权限授予状态列表。</returns>
    Task<List<IsGrantedResponse>> IsGrantedAsync(List<IsGrantedRequest> requests);
}
