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
[Route("api/localization-management/resources")]
public class LocalizationResourceController : AbpControllerBase, ILocalizationResourceAppService
{
    private readonly ILocalizationResourceAppService _appService;

    public LocalizationResourceController(ILocalizationResourceAppService appService)
    {
        _appService = appService;
    }

    [HttpGet]
    public Task<List<LocalizationResourceDto>> GetAllAsync()
    {
        return _appService.GetAllAsync();
    }

    [HttpPost]
    public Task<LocalizationResourceDto> CreateAsync(CreateUpdateLocalizationResourceDto input)
    {
        return _appService.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public Task<LocalizationResourceDto> UpdateAsync(Guid id, CreateUpdateLocalizationResourceDto input)
    {
        return _appService.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _appService.DeleteAsync(id);
    }
}
