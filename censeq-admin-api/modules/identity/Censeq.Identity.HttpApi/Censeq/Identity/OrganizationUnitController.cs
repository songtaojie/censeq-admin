using System;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace Censeq.Identity;

[RemoteService(Name = IdentityRemoteServiceConsts.RemoteServiceName)]
[Area(IdentityRemoteServiceConsts.ModuleName)]
[ControllerName("OrganizationUnit")]
[Route("api/identity/organization-units")]
public class OrganizationUnitController : AbpControllerBase, IOrganizationUnitAppService
{
    protected IOrganizationUnitAppService OrganizationUnitAppService { get; }

    public OrganizationUnitController(IOrganizationUnitAppService organizationUnitAppService)
    {
        OrganizationUnitAppService = organizationUnitAppService;
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<OrganizationUnitDto> GetAsync(Guid id)
    {
        return OrganizationUnitAppService.GetAsync(id);
    }

    [HttpGet]
    public virtual Task<ListResultDto<OrganizationUnitDto>> GetAllListAsync()
    {
        return OrganizationUnitAppService.GetAllListAsync();
    }

    [HttpPost]
    public virtual Task<OrganizationUnitDto> CreateAsync(OrganizationUnitCreateDto input)
    {
        return OrganizationUnitAppService.CreateAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    public virtual Task<OrganizationUnitDto> UpdateAsync(Guid id, OrganizationUnitUpdateDto input)
    {
        return OrganizationUnitAppService.UpdateAsync(id, input);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return OrganizationUnitAppService.DeleteAsync(id);
    }
}
