using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace Censeq.PermissionManagement;

/// <summary>
/// 权限定义 HTTP API 控制器。
/// </summary>
[Controller]
[RemoteService(Name = PermissionManagementRemoteServiceConsts.RemoteServiceName)]
[Area(PermissionManagementRemoteServiceConsts.ModuleName)]
[Route("api/permission-management/definition")]
public class PermissionDefinitionController : AbpControllerBase, IPermissionDefinitionAppService
{
    private readonly IPermissionDefinitionAppService _appService;

    /// <summary>
    /// 初始化 PermissionDefinitionController 实例。
    /// </summary>
    /// <param name="appService">权限定义应用服务。</param>
    public PermissionDefinitionController(IPermissionDefinitionAppService appService)
    {
        _appService = appService;
    }

    /// <summary>
    /// 获取权限组定义列表。
    /// </summary>
    /// <returns>权限组定义列表。</returns>
    [HttpGet("groups")]
    public Task<List<PermissionGroupDefinitionDto>> GetGroupsAsync()
        => _appService.GetGroupsAsync();

    /// <summary>
    /// 更新权限组显示名称。
    /// </summary>
    /// <param name="groupName">权限组名称。</param>
    /// <param name="input">更新请求数据。</param>
    /// <returns>更新后的权限组定义。</returns>
    [HttpPut("groups/{groupName}")]
    public Task<PermissionGroupDefinitionDto> UpdateGroupAsync(string groupName, UpdatePermissionGroupDefinitionDto input)
        => _appService.UpdateGroupAsync(groupName, input);

    /// <summary>
    /// 获取指定权限组下的权限定义列表。
    /// </summary>
    /// <param name="groupName">权限组名称。</param>
    /// <returns>权限定义列表。</returns>
    [HttpGet("groups/{groupName}/permissions")]
    public Task<List<PermissionDefinitionDto>> GetPermissionsAsync(string groupName)
        => _appService.GetPermissionsAsync(groupName);

    /// <summary>
    /// 更新权限定义显示名称。
    /// </summary>
    /// <param name="name">权限名称。</param>
    /// <param name="input">更新请求数据。</param>
    /// <returns>更新后的权限定义。</returns>
    [HttpPut("permissions/{name}")]
    public Task<PermissionDefinitionDto> UpdatePermissionAsync(string name, UpdatePermissionDefinitionDto input)
        => _appService.UpdatePermissionAsync(name, input);
}
