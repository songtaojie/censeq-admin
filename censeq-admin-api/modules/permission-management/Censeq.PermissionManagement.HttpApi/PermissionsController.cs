using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace Censeq.PermissionManagement;

/// <summary>
/// 权限管理 HTTP API。
/// 对外暴露权限查询和批量更新接口，实际业务逻辑由应用服务完成。
/// </summary>
[Controller]
[RemoteService(Name = PermissionManagementRemoteServiceConsts.RemoteServiceName)]
[Area(PermissionManagementRemoteServiceConsts.ModuleName)]
[Route("api/permission-management/permissions")]
public class PermissionsController : AbpControllerBase, IPermissionAppService
{
    /// <summary>
    /// 权限应用服务。
    /// </summary>
    protected IPermissionAppService PermissionAppService { get; }

    /// <summary>
    /// 初始化权限管理控制器。
    /// </summary>
    /// <param name="permissionAppService">权限应用服务。</param>
    public PermissionsController(IPermissionAppService permissionAppService)
    {
        PermissionAppService = permissionAppService;
    }

    /// <summary>
    /// 获取指定提供者的权限授予列表。
    /// </summary>
    /// <param name="providerName">权限提供者名称。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    /// <returns>权限列表结果。</returns>
    [HttpGet]
    public virtual Task<GetPermissionListResultDto> GetAsync(string providerName, string providerKey)
    {
        return PermissionAppService.GetAsync(providerName, providerKey);
    }

    /// <summary>
    /// 批量更新指定提供者的权限授予状态。
    /// </summary>
    /// <param name="providerName">权限提供者名称。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    /// <param name="input">权限更新内容。</param>
    /// <returns>异步任务。</returns>
    [HttpPut]
    public virtual Task UpdateAsync(string providerName, string providerKey, UpdatePermissionsDto input)
    {
        return PermissionAppService.UpdateAsync(providerName, providerKey, input);
    }
}
