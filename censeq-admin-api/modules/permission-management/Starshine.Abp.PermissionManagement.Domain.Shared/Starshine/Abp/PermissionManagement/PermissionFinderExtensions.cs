using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Censeq.Abp.PermissionManagement;

/// <summary>
/// Ȩ�޲�������չ
/// </summary>
public static class PermissionFinderExtensions
{
    /// <summary>
    /// �Ƿ�����Ȩ
    /// </summary>
    /// <param name="permissionFinder">Ȩ�޲�����</param>
    /// <param name="userId">�û�id</param>
    /// <param name="permissionName">Ȩ������</param>
    /// <returns></returns>
    public async static Task<bool> IsGrantedAsync(this IPermissionFinder permissionFinder, Guid userId, string permissionName)
    {
        return await permissionFinder.IsGrantedAsync(userId, new[] { permissionName });
    }

    /// <summary>
    /// �Ƿ�����Ȩ
    /// </summary>
    /// <param name="permissionFinder">Ȩ�޲�����</param>
    /// <param name="userId">�û�id</param>
    /// <param name="permissionNames">Ȩ������</param>
    /// <returns></returns>
    public async static Task<bool> IsGrantedAsync(this IPermissionFinder permissionFinder, Guid userId, string[] permissionNames)
    {
        return (await permissionFinder.IsGrantedAsync(
        [
            new IsGrantedRequest
            {
                UserId = userId,
                PermissionNames = permissionNames
            }
        ])).Any(x => x.UserId == userId && x.Permissions.All(p => permissionNames.Contains(p.Key) && p.Value));
    }
}
