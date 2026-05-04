using System;
using System.Threading.Tasks;
using Censeq.LocalizationManagement.Dtos;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace Censeq.LocalizationManagement;

[RemoteService(Name = LocalizationManagementRemoteServiceConsts.RemoteServiceName)]
[Area(LocalizationManagementRemoteServiceConsts.ModuleName)]
[Route("api/localization-management/texts")]
public class LocalizationTextController : AbpControllerBase, ILocalizationTextAppService
{
    private readonly ILocalizationTextAppService _appService;

    public LocalizationTextController(ILocalizationTextAppService appService)
    {
        _appService = appService;
    }

    [HttpGet]
    public Task<PagedResultDto<LocalizationTextDto>> GetListAsync([FromQuery] GetLocalizationTextsInput input)
    {
        return _appService.GetListAsync(input);
    }

    [HttpGet("{id}")]
    public Task<LocalizationTextDto> GetAsync(Guid id)
    {
        return _appService.GetAsync(id);
    }

    [HttpPost]
    public Task<LocalizationTextDto> CreateAsync(CreateLocalizationTextDto input)
    {
        return _appService.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public Task<LocalizationTextDto> UpdateAsync(Guid id, UpdateLocalizationTextDto input)
    {
        return _appService.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _appService.DeleteAsync(id);
    }
}
