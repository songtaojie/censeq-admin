using System;
using System.Linq;
using System.Threading.Tasks;
using Censeq.Identity;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Security.Claims;

namespace Censeq.PermissionManagement.Identity;

/// <summary>
/// 基于角色 ID 的角色权限值提供器。
/// 替换 ABP 默认的角色权限值提供器，使运行时权限检查使用角色 ID 作为授权 ProviderKey。
/// </summary>
[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(RolePermissionValueProvider), typeof(IPermissionValueProvider))]
public class RoleIdPermissionValueProvider : RolePermissionValueProvider
{
    /// <summary>
    /// 用户角色查找器，用于获取当前用户拥有的角色 ID。
    /// </summary>
    protected IUserRoleFinder UserRoleFinder { get; }

    /// <summary>
    /// 初始化基于角色 ID 的角色权限值提供器。
    /// </summary>
    /// <param name="permissionStore">权限授权存储。</param>
    /// <param name="userRoleFinder">用户角色查找器。</param>
    public RoleIdPermissionValueProvider(
        IPermissionStore permissionStore,
        IUserRoleFinder userRoleFinder)
        : base(permissionStore)
    {
        UserRoleFinder = userRoleFinder;
    }

    /// <summary>
    /// 检查当前用户是否通过任一角色 ID 被授予指定权限。
    /// </summary>
    /// <param name="context">单个权限检查上下文。</param>
    /// <returns>权限检查结果。</returns>
    public override async Task<PermissionGrantResult> CheckAsync(PermissionValueCheckContext context)
    {
        var userId = GetUserIdOrNull(context);
        if (!userId.HasValue)
        {
            return PermissionGrantResult.Undefined;
        }

        var roleIds = await UserRoleFinder.GetRoleIdsAsync(userId.Value);
        foreach (var roleId in roleIds)
        {
            if (await PermissionStore.IsGrantedAsync(context.Permission.Name, Name, roleId.ToString()))
            {
                return PermissionGrantResult.Granted;
            }
        }

        return PermissionGrantResult.Undefined;
    }

    /// <summary>
    /// 批量检查当前用户是否通过任一角色 ID 被授予指定权限集合。
    /// </summary>
    /// <param name="context">批量权限检查上下文。</param>
    /// <returns>批量权限检查结果。</returns>
    public override async Task<MultiplePermissionGrantResult> CheckAsync(PermissionValuesCheckContext context)
    {
        var result = new MultiplePermissionGrantResult();
        var permissionNames = context.Permissions.Select(x => x.Name).ToArray();
        var userId = GetUserIdOrNull(context);
        if (!userId.HasValue)
        {
            permissionNames.ToList().ForEach(x => result.Result.Add(x, PermissionGrantResult.Undefined));
            return result;
        }

        foreach (var permissionName in permissionNames)
        {
            result.Result.Add(permissionName, PermissionGrantResult.Undefined);
        }

        var roleIds = await UserRoleFinder.GetRoleIdsAsync(userId.Value);
        foreach (var roleId in roleIds)
        {
            var roleResult = await PermissionStore.IsGrantedAsync(permissionNames, Name, roleId.ToString());
            foreach (var permissionName in permissionNames)
            {
                if (roleResult.Result.GetValueOrDefault(permissionName) == PermissionGrantResult.Granted)
                {
                    result.Result[permissionName] = PermissionGrantResult.Granted;
                }
            }
        }

        return result;
    }

    /// <summary>
    /// 从单个权限检查上下文中读取当前用户 ID。
    /// </summary>
    /// <param name="context">单个权限检查上下文。</param>
    /// <returns>当前用户 ID；无法解析时返回 null。</returns>
    protected virtual Guid? GetUserIdOrNull(PermissionValueCheckContext context)
    {
        var userIdValue = context.Principal?.FindFirst(AbpClaimTypes.UserId)?.Value;
        return Guid.TryParse(userIdValue, out var userId) ? userId : null;
    }

    /// <summary>
    /// 从批量权限检查上下文中读取当前用户 ID。
    /// </summary>
    /// <param name="context">批量权限检查上下文。</param>
    /// <returns>当前用户 ID；无法解析时返回 null。</returns>
    protected virtual Guid? GetUserIdOrNull(PermissionValuesCheckContext context)
    {
        var userIdValue = context.Principal?.FindFirst(AbpClaimTypes.UserId)?.Value;
        return Guid.TryParse(userIdValue, out var userId) ? userId : null;
    }
}
