using System.Linq;
using System.Threading.Tasks;

namespace Censeq.PermissionManagement;

/// <summary>
/// 权限查询扩展方法。
/// </summary>
public static class PermissionFinderExtensions
{
    /// <summary>
    /// 检查用户是否已授予指定权限。
    /// </summary>
    /// <param name="permissionFinder">权限查询器。</param>
    /// <param name="userId">用户标识。</param>
    /// <param name="permissionName">权限名称。</param>
    /// <returns>已授予时返回 true，否则返回 false。</returns>
    public static async Task<bool> IsGrantedAsync(this IPermissionFinder permissionFinder, Guid userId, string permissionName)
    {
        return await permissionFinder.IsGrantedAsync(userId, new[] { permissionName });
    }

    /// <summary>
    /// 检查用户是否已授予指定权限。
    /// </summary>
    /// <param name="permissionFinder">权限查询器。</param>
    /// <param name="userId">用户标识。</param>
    /// <param name="permissionNames">权限名称集合。</param>
    /// <returns>已授予时返回 true，否则返回 false。</returns>
    public static async Task<bool> IsGrantedAsync(this IPermissionFinder permissionFinder, Guid userId, string[] permissionNames)
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
