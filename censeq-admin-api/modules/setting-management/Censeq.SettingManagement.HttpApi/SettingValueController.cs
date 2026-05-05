using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace Censeq.SettingManagement;

[RemoteService(Name = SettingManagementRemoteServiceConsts.RemoteServiceName)]
[Area(SettingManagementRemoteServiceConsts.ModuleName)]
[Route("api/setting-management/setting-values")]
public class SettingValueController : AbpControllerBase, ISettingValueAppService
{
    private readonly ISettingValueAppService _appService;

    public SettingValueController(ISettingValueAppService appService)
    {
        _appService = appService;
    }

    [HttpGet]
    public virtual Task<SettingValueDto> GetAsync([FromQuery] string name)
    {
        return _appService.GetAsync(name);
    }

    [HttpPut]
    public virtual Task SetAsync([FromBody] SetSettingValueInput input)
    {
        return _appService.SetAsync(input);
    }

    [HttpDelete]
    public virtual Task DeleteAsync(
        [FromQuery] string name,
        [FromQuery] string providerName,
        [FromQuery] string? providerKey)
    {
        return _appService.DeleteAsync(name, providerName, providerKey);
    }
}
