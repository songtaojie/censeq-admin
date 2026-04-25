using System;
using System.Threading.Tasks;

namespace Censeq.Identity;

/// <summary>
/// 用户角色查找器接口
/// </summary>
public interface IUserRoleFinder
{
    /// <summary>
    /// 获取用户角色（已过时，请使用 GetRoleNamesAsync）
    /// </summary>
    /// <param name="userId">用户标识</param>
    /// <returns>角色数组</returns>
    [Obsolete("Use GetRoleNamesAsync instead.")]
    Task<string[]> GetRolesAsync(Guid userId);

    /// <summary>
    /// 异步获取用户角色名称列表
    /// </summary>
    /// <param name="userId">用户标识</param>
    /// <returns>角色名称数组</returns>
    Task<string[]> GetRoleNamesAsync(Guid userId);
}
