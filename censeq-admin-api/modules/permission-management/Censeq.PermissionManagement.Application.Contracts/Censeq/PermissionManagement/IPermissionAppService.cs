using JetBrains.Annotations;
using Volo.Abp.Application.Services;

namespace Censeq.PermissionManagement;

/// <summary>
/// 权限应用服务契约。
/// 用于查询和更新角色、用户等权限提供者上的授权状态。
/// </summary>
public interface IPermissionAppService : IApplicationService
{
    /// <summary>
    /// 获取指定提供者的权限授予列表。
    /// </summary>
    /// <param name="providerName">权限提供者名称。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    /// <returns>按权限组组织的权限授予结果。</returns>
    Task<GetPermissionListResultDto> GetAsync([NotNull] string providerName, [NotNull] string providerKey);

    /// <summary>
    /// 批量更新指定提供者的权限授予状态。
    /// </summary>
    /// <param name="providerName">权限提供者名称。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    /// <param name="input">权限更新内容。</param>
    /// <returns>异步任务。</returns>
    Task UpdateAsync([NotNull] string providerName, [NotNull] string providerKey, UpdatePermissionsDto input);
}
