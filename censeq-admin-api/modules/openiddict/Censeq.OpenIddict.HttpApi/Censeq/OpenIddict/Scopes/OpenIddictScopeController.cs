using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Censeq.OpenIddict.Scopes;

[RemoteService(Name = OpenIddictRemoteServiceConsts.RemoteServiceName)]
[Area(OpenIddictRemoteServiceConsts.ModuleName)]
[Route("api/openIddict/scopes")]
public class OpenIddictScopeController : OpenIddictControllerBase, IOpenIddictScopeAppService
{
    protected IOpenIddictScopeAppService ScopeAppService { get; }

    public OpenIddictScopeController(IOpenIddictScopeAppService scopeAppService)
    {
        ScopeAppService = scopeAppService;
    }

    [HttpGet]
    public virtual Task<PagedResultDto<OpenIddictScopeDto>> GetListAsync(GetOpenIddictScopesInput input)
    {
        return ScopeAppService.GetListAsync(input);
    }

    [HttpGet("{id}")]
    public virtual Task<OpenIddictScopeDto> GetAsync(Guid id)
    {
        return ScopeAppService.GetAsync(id);
    }

    [HttpPost]
    public virtual Task<OpenIddictScopeDto> CreateAsync(OpenIddictScopeCreateDto input)
    {
        return ScopeAppService.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public virtual Task<OpenIddictScopeDto> UpdateAsync(Guid id, OpenIddictScopeUpdateDto input)
    {
        return ScopeAppService.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return ScopeAppService.DeleteAsync(id);
    }

    [HttpGet("check-name")]
    public virtual Task<bool> CheckNameExistsAsync(string name, Guid? excludeId = null)
    {
        return ScopeAppService.CheckNameExistsAsync(name, excludeId);
    }
}
