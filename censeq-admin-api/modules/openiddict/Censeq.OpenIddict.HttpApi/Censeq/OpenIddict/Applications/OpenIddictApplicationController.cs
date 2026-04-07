using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Censeq.OpenIddict.Applications;

[RemoteService(Name = OpenIddictRemoteServiceConsts.RemoteServiceName)]
[Area(OpenIddictRemoteServiceConsts.ModuleName)]
[Route("api/openIddict/applications")]
public class OpenIddictApplicationController : OpenIddictControllerBase, IOpenIddictApplicationAppService
{
    protected IOpenIddictApplicationAppService ApplicationAppService { get; }

    public OpenIddictApplicationController(IOpenIddictApplicationAppService applicationAppService)
    {
        ApplicationAppService = applicationAppService;
    }

    [HttpGet]
    public virtual Task<PagedResultDto<OpenIddictApplicationDto>> GetListAsync(GetOpenIddictApplicationsInput input)
    {
        return ApplicationAppService.GetListAsync(input);
    }

    [HttpGet("{id}")]
    public virtual Task<OpenIddictApplicationDto> GetAsync(Guid id)
    {
        return ApplicationAppService.GetAsync(id);
    }

    [HttpPost]
    public virtual Task<OpenIddictApplicationDto> CreateAsync(OpenIddictApplicationCreateDto input)
    {
        return ApplicationAppService.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public virtual Task<OpenIddictApplicationDto> UpdateAsync(Guid id, OpenIddictApplicationUpdateDto input)
    {
        return ApplicationAppService.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return ApplicationAppService.DeleteAsync(id);
    }

    [HttpPost("{id}/generate-secret")]
    public virtual Task<string> GenerateClientSecretAsync(Guid id)
    {
        return ApplicationAppService.GenerateClientSecretAsync(id);
    }

    [HttpGet("check-client-id")]
    public virtual Task<bool> CheckClientIdExistsAsync(string clientId, Guid? excludeId = null)
    {
        return ApplicationAppService.CheckClientIdExistsAsync(clientId, excludeId);
    }
}
