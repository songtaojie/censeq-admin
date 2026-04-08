using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Censeq.Identity.Entities;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Censeq.Identity;

[Authorize(IdentityPermissions.ClaimTypes.Default)]
public class IdentityClaimTypeAppService : IdentityAppServiceBase, IIdentityClaimTypeAppService
{
    protected IIdentityClaimTypeRepository ClaimTypeRepository { get; }
    protected IdentityClaimTypeManager ClaimTypeManager { get; }

    public IdentityClaimTypeAppService(
        IIdentityClaimTypeRepository claimTypeRepository,
        IdentityClaimTypeManager claimTypeManager)
    {
        ClaimTypeRepository = claimTypeRepository;
        ClaimTypeManager = claimTypeManager;
    }

    public virtual async Task<IdentityClaimTypeDto> GetAsync(Guid id)
    {
        return MapToDto(await ClaimTypeRepository.GetAsync(id));
    }

    public virtual async Task<ListResultDto<IdentityClaimTypeDto>> GetAllListAsync()
    {
        var list = await ClaimTypeRepository.GetListAsync(
            nameof(IdentityClaimType.Name),
            int.MaxValue,
            0,
            string.Empty);

        return new ListResultDto<IdentityClaimTypeDto>(MapToDtoList(list));
    }

    public virtual async Task<PagedResultDto<IdentityClaimTypeDto>> GetListAsync(GetIdentityClaimTypesInput input)
    {
        var list = await ClaimTypeRepository.GetListAsync(
            input.Sorting,
            input.MaxResultCount,
            input.SkipCount,
            input.Filter ?? string.Empty);

        var totalCount = await ClaimTypeRepository.GetCountAsync(input.Filter);

        return new PagedResultDto<IdentityClaimTypeDto>(totalCount, MapToDtoList(list));
    }

    [Authorize(IdentityPermissions.ClaimTypes.Create)]
    public virtual async Task<IdentityClaimTypeDto> CreateAsync(IdentityClaimTypeCreateDto input)
    {
        var claimType = new IdentityClaimType(
            GuidGenerator.Create(),
            input.Name,
            input.Required,
            input.IsStatic,
            input.Regex,
            input.RegexDescription,
            input.Description,
            ParseValueType(input.ValueType));

        claimType = await ClaimTypeManager.CreateAsync(claimType);

        return MapToDto(claimType);
    }

    [Authorize(IdentityPermissions.ClaimTypes.Update)]
    public virtual async Task<IdentityClaimTypeDto> UpdateAsync(Guid id, IdentityClaimTypeUpdateDto input)
    {
        var claimType = await ClaimTypeRepository.GetAsync(id);
        claimType.SetName(input.Name);
        claimType.Required = input.Required;
        claimType.Regex = input.Regex;
        claimType.RegexDescription = input.RegexDescription;
        claimType.Description = input.Description;
        claimType.ValueType = ParseValueType(input.ValueType);

        claimType = await ClaimTypeManager.UpdateAsync(claimType);

        return MapToDto(claimType);
    }

    [Authorize(IdentityPermissions.ClaimTypes.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        await ClaimTypeManager.DeleteAsync(id);
    }

    protected virtual IdentityClaimTypeDto MapToDto(IdentityClaimType claimType)
    {
        return new IdentityClaimTypeDto
        {
            Id = claimType.Id,
            Name = claimType.Name,
            Required = claimType.Required,
            IsStatic = claimType.IsStatic,
            Regex = claimType.Regex,
            RegexDescription = claimType.RegexDescription,
            Description = claimType.Description,
            ValueType = claimType.ValueType.ToString()
        };
    }

    protected virtual List<IdentityClaimTypeDto> MapToDtoList(List<IdentityClaimType> claimTypes)
    {
        var list = new List<IdentityClaimTypeDto>();
        foreach (var claimType in claimTypes)
        {
            list.Add(MapToDto(claimType));
        }

        return list;
    }

    protected virtual IdentityClaimValueType ParseValueType(string valueType)
    {
        return Enum.Parse<IdentityClaimValueType>(valueType, ignoreCase: true);
    }
}