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
[ControllerName("Role")]
[Route("api/identity/roles")]
public class IdentityRoleController : AbpControllerBase, IIdentityRoleAppService
{
    protected IIdentityRoleAppService RoleAppService { get; }

    public IdentityRoleController(IIdentityRoleAppService roleAppService)
    {
        RoleAppService = roleAppService;
    }

    [HttpGet]
    [Route("all")]
    public virtual Task<ListResultDto<IdentityRoleDto>> GetAllListAsync()
    {
        return RoleAppService.GetAllListAsync();
    }

    [HttpGet]
    public virtual Task<PagedResultDto<IdentityRoleDto>> GetListAsync(GetIdentityRolesInput input)
    {
        return RoleAppService.GetListAsync(input);
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<IdentityRoleDto> GetAsync(Guid id)
    {
        return RoleAppService.GetAsync(id);
    }

    [HttpPost]
    public virtual Task<IdentityRoleDto> CreateAsync(IdentityRoleCreateDto input)
    {
        return RoleAppService.CreateAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    public virtual Task<IdentityRoleDto> UpdateAsync(Guid id, IdentityRoleUpdateDto input)
    {
        return RoleAppService.UpdateAsync(id, input);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return RoleAppService.DeleteAsync(id);
    }

    [HttpGet]
    [Route("{roleId}/claims")]
    public virtual Task<IdentityRoleClaimListDto> GetClaimsAsync(Guid roleId)
    {
        return RoleAppService.GetClaimsAsync(roleId);
    }

    [HttpPost]
    [Route("{roleId}/claims")]
    public virtual Task AddClaimAsync(Guid roleId, [FromBody] IdentityRoleClaimCreateDto input)
    {
        return RoleAppService.AddClaimAsync(roleId, input);
    }

    [HttpDelete]
    [Route("{roleId}/claims/{claimId}")]
    public virtual Task RemoveClaimAsync(Guid roleId, Guid claimId)
    {
        return RoleAppService.RemoveClaimAsync(roleId, claimId);
    }
}
