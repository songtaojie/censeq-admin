using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace Censeq.Identity;

[RemoteService(Name = IdentityRemoteServiceConsts.RemoteServiceName)]
[Area(IdentityRemoteServiceConsts.ModuleName)]
[Route("api/identity/claim-types")]
public class IdentityClaimTypeController : AbpControllerBase, IIdentityClaimTypeAppService
{
    protected IIdentityClaimTypeAppService ClaimTypeAppService { get; }

    public IdentityClaimTypeController(IIdentityClaimTypeAppService claimTypeAppService)
    {
        ClaimTypeAppService = claimTypeAppService;
    }

    [HttpGet("all")]
    public virtual Task<ListResultDto<IdentityClaimTypeDto>> GetAllListAsync()
    {
        return ClaimTypeAppService.GetAllListAsync();
    }

    [HttpGet]
    public virtual Task<PagedResultDto<IdentityClaimTypeDto>> GetListAsync(GetIdentityClaimTypesInput input)
    {
        return ClaimTypeAppService.GetListAsync(input);
    }

    [HttpGet("{id}")]
    public virtual Task<IdentityClaimTypeDto> GetAsync(Guid id)
    {
        return ClaimTypeAppService.GetAsync(id);
    }

    [HttpPost]
    public virtual Task<IdentityClaimTypeDto> CreateAsync(IdentityClaimTypeCreateDto input)
    {
        return ClaimTypeAppService.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public virtual Task<IdentityClaimTypeDto> UpdateAsync(Guid id, IdentityClaimTypeUpdateDto input)
    {
        return ClaimTypeAppService.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return ClaimTypeAppService.DeleteAsync(id);
    }
}