using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Censeq.Identity.Entities;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;

namespace Censeq.Identity;

[Authorize(IdentityPermissions.OrganizationUnits.Default)]
/// <summary>
/// 组织单元应用服务
/// </summary>
public class OrganizationUnitAppService : IdentityAppServiceBase, IOrganizationUnitAppService
{
    /// <summary>
    /// I组织单元仓储
    /// </summary>
    protected IOrganizationUnitRepository OrganizationUnitRepository { get; }
    /// <summary>
    /// 组织单元管理器
    /// </summary>
    protected OrganizationUnitManager OrganizationUnitManager { get; }

    public OrganizationUnitAppService(
        IOrganizationUnitRepository organizationUnitRepository,
        OrganizationUnitManager organizationUnitManager)
    {
        OrganizationUnitRepository = organizationUnitRepository;
        OrganizationUnitManager = organizationUnitManager;
    }

    /// <summary>
    /// Task<Organization单元Dto>
    /// </summary>
    public virtual async Task<OrganizationUnitDto> GetAsync(Guid id)
    {
        var entity = await OrganizationUnitRepository.GetAsync(id);
        return ObjectMapper.Map<OrganizationUnit, OrganizationUnitDto>(entity);
    }

    /// <summary>
    /// Task<List结果Dto<Organization单元Dto>>
    /// </summary>
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
    /// <summary>
    /// Task<Organization单元Dto>
    /// </summary>
    public virtual async Task<OrganizationUnitDto> CreateAsync(OrganizationUnitCreateDto input)
    {
        var entity = new OrganizationUnit(
            GuidGenerator.Create(),
            input.DisplayName,
            input.ParentId,
            CurrentTenant.Id);

        entity.Status = input.Status;
        entity.Remark = input.Remark;

        await OrganizationUnitManager.CreateAsync(entity);
        await CurrentUnitOfWork!.SaveChangesAsync();

        return ObjectMapper.Map<OrganizationUnit, OrganizationUnitDto>(entity);
    }

    [Authorize(IdentityPermissions.OrganizationUnits.Update)]
    /// <summary>
    /// Task<Organization单元Dto>
    /// </summary>
    public virtual async Task<OrganizationUnitDto> UpdateAsync(Guid id, OrganizationUnitUpdateDto input)
    {
        var entity = await OrganizationUnitRepository.GetAsync(id);
        entity.DisplayName = input.DisplayName;
        entity.Status = input.Status;
        entity.Remark = input.Remark;
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
