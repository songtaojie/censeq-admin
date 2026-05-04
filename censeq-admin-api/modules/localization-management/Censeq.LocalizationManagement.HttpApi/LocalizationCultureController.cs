using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Censeq.LocalizationManagement.Dtos;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace Censeq.LocalizationManagement;

[RemoteService(Name = LocalizationManagementRemoteServiceConsts.RemoteServiceName)]
[Area(LocalizationManagementRemoteServiceConsts.ModuleName)]
[Route("api/localization-management/cultures")]
public class LocalizationCultureController : AbpControllerBase, ILocalizationCultureAppService
{
    private readonly ILocalizationCultureAppService _appService;

    public LocalizationCultureController(ILocalizationCultureAppService appService)
    {
        _appService = appService;
    }

    [HttpGet]
    public Task<List<LocalizationCultureDto>> GetListAsync()
    {
        return _appService.GetListAsync();
    }

    [HttpPost]
    public Task<LocalizationCultureDto> CreateAsync(CreateUpdateLocalizationCultureDto input)
    {
        return _appService.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public Task<LocalizationCultureDto> UpdateAsync(Guid id, CreateUpdateLocalizationCultureDto input)
    {
        return _appService.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _appService.DeleteAsync(id);
    }
}
