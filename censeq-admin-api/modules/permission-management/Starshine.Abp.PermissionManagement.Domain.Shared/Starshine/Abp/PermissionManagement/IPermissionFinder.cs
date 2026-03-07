using System.Collections.Generic;
using System.Threading.Tasks;

namespace Censeq.Abp.PermissionManagement;

/// <summary>
/// Ȩ�޲�ѯ
/// </summary>
public interface IPermissionFinder
{
    /// <summary>
    /// �Ƿ���Ȩ
    /// </summary>
    /// <param name="requests">����������</param>
    /// <returns></returns>
    Task<List<IsGrantedResponse>> IsGrantedAsync(List<IsGrantedRequest> requests);
}
