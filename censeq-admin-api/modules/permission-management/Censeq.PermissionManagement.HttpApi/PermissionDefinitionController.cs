using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace Censeq.PermissionManagement;

[Controller]
[RemoteService(Name = PermissionManagementRemoteServiceConsts.RemoteServiceName)]
[Area(PermissionManagementRemoteServiceConsts.ModuleName)]
[Route("api/permission-management/definition")]
public class PermissionDefinitionController : AbpControllerBase, IPermissionDefinitionAppService
{
    private readonly IPermissionDefinitionAppService _appService;

    public PermissionDefinitionController(IPermissionDefinitionAppService appService)
    {
        _appService = appService;
    }

    [HttpGet("groups")]
    public Task<List<PermissionGroupDefinitionDto>> GetGroupsAsync()
        => _appService.GetGroupsAsync();

    [HttpPut("groups/{groupName}")]
    public Task<PermissionGroupDefinitionDto> UpdateGroupAsync(string groupName, UpdatePermissionGroupDefinitionDto input)
        => _appService.UpdateGroupAsync(groupName, input);

    [HttpGet("groups/{groupName}/permissions")]
    public Task<List<PermissionDefinitionDto>> GetPermissionsAsync(string groupName)
        => _appService.GetPermissionsAsync(groupName);

    [HttpPut("permissions/{name}")]
    public Task<PermissionDefinitionDto> UpdatePermissionAsync(string name, UpdatePermissionDefinitionDto input)
        => _appService.UpdatePermissionAsync(name, input);
}
