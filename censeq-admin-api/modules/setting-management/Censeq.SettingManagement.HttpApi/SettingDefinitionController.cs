using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace Censeq.SettingManagement;

[RemoteService(Name = SettingManagementRemoteServiceConsts.RemoteServiceName)]
[Area(SettingManagementRemoteServiceConsts.ModuleName)]
[Route("api/setting-management/setting-definitions")]
public class SettingDefinitionController : AbpControllerBase, ISettingDefinitionAppService
{
    private readonly ISettingDefinitionAppService _appService;

    public SettingDefinitionController(ISettingDefinitionAppService appService)
    {
        _appService = appService;
    }

    [HttpGet]
    public virtual Task<PagedResultDto<SettingDefinitionDto>> GetListAsync([FromQuery] SettingDefinitionGetListInput input)
    {
        return _appService.GetListAsync(input);
    }

    [HttpGet("{id:guid}")]
    public virtual Task<SettingDefinitionDto> GetAsync(Guid id)
    {
        return _appService.GetAsync(id);
    }

    [HttpPost]
    public virtual Task<SettingDefinitionDto> CreateAsync([FromBody] CreateSettingDefinitionDto input)
    {
        return _appService.CreateAsync(input);
    }

    [HttpPut("{id:guid}")]
    public virtual Task<SettingDefinitionDto> UpdateAsync(Guid id, [FromBody] UpdateSettingDefinitionDto input)
    {
        return _appService.UpdateAsync(id, input);
    }

    [HttpDelete("{id:guid}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return _appService.DeleteAsync(id);
    }
}
