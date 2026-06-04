using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using Censeq.PermissionManagement;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace Censeq.PermissionManagement.Integration;

/// <summary>
/// 权限集成 HTTP API 控制器。
/// </summary>
[RemoteService(Name = PermissionManagementRemoteServiceConsts.RemoteServiceName)]
[Area(PermissionManagementRemoteServiceConsts.ModuleName)]
[ControllerName("PermissionIntegration")]
[Route("integration-api/permission-management/permissions")]
public class PermissionIntegrationController : AbpControllerBase, IPermissionIntegrationService
{
    /// <summary>
    /// 权限集成应用服务。
    /// </summary>
    protected IPermissionIntegrationService PermissionIntegrationService { get; }

    /// <summary>
    /// 初始化 PermissionIntegrationController 实例。
    /// </summary>
    /// <param name="permissionIntegrationService">权限集成应用服务。</param>
    public PermissionIntegrationController(IPermissionIntegrationService permissionIntegrationService)
    {
        PermissionIntegrationService = permissionIntegrationService;
    }

    /// <summary>
    /// 检查用户是否已授予指定权限。
    /// </summary>
    /// <param name="input">更新请求数据。</param>
    /// <returns>已授予时返回 true，否则返回 false。</returns>
    [HttpGet]
    [Route("is-granted")]
    public virtual Task<ListResultDto<IsGrantedResponse>> IsGrantedAsync(List<IsGrantedRequest> input)
    {
        return PermissionIntegrationService.IsGrantedAsync(input);
    }
}
