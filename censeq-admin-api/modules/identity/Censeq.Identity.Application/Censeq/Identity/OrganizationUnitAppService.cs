using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;

namespace Censeq.Identity;

[Authorize(IdentityPermissions.OrganizationUnits.Default)]
public class OrganizationUnitAppService : IdentityAppServiceBase, IOrganizationUnitAppService
{
    protected IOrganizationUnitRepository OrganizationUnitRepository { get; }
    protected OrganizationUnitManager OrganizationUnitManager { get; }

    public OrganizationUnitAppService(
        IOrganizationUnitRepository organizationUnitRepository,
        OrganizationUnitManager organizationUnitManager)
    {
        OrganizationUnitRepository = organizationUnitRepository;
        OrganizationUnitManager = organizationUnitManager;
    }

    public virtual async Task<OrganizationUnitDto> GetAsync(Guid id)
    {
        var entity = await OrganizationUnitRepository.GetAsync(id);
        return ObjectMapper.Map<OrganizationUnit, OrganizationUnitDto>(entity);
    }

    public virtual async Task<ListResultDto<OrganizationUnitDto>> GetAllListAsync()
    {
        var list = await OrganizationUnitRepository.GetListAsync(
            sorting: nameof(OrganizationUnit.Code),
            maxResultCount: int.MaxValue,
            skipCount: 0,
            includeDetails: false);

        return new ListResultDto<OrganizationUnitDto>(
            ObjectMapper.Map<List<OrganizationUnit>, List<OrganizationUnitDto>>(list));
    }

    [Authorize(IdentityPermissions.OrganizationUnits.Create)]
    public virtual async Task<OrganizationUnitDto> CreateAsync(OrganizationUnitCreateDto input)
    {
        var entity = new OrganizationUnit(
            GuidGenerator.Create(),
            input.DisplayName,
            input.ParentId,
            CurrentTenant.Id);

        await OrganizationUnitManager.CreateAsync(entity);
        await CurrentUnitOfWork!.SaveChangesAsync();

        return ObjectMapper.Map<OrganizationUnit, OrganizationUnitDto>(entity);
    }

    [Authorize(IdentityPermissions.OrganizationUnits.Update)]
    public virtual async Task<OrganizationUnitDto> UpdateAsync(Guid id, OrganizationUnitUpdateDto input)
    {
        var entity = await OrganizationUnitRepository.GetAsync(id);
        entity.DisplayName = input.DisplayName;
        await OrganizationUnitManager.UpdateAsync(entity);
        await CurrentUnitOfWork!.SaveChangesAsync();

        return ObjectMapper.Map<OrganizationUnit, OrganizationUnitDto>(entity);
    }

    [Authorize(IdentityPermissions.OrganizationUnits.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        await OrganizationUnitManager.DeleteAsync(id);
    }
}
