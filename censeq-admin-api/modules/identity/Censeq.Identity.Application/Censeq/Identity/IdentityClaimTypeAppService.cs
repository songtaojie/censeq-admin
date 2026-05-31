using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Censeq.Identity.Entities;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Censeq.Identity;

[Authorize(IdentityPermissions.ClaimTypes.Default)]
/// <summary>
/// 身份声明类型应用服务
/// </summary>
public class IdentityClaimTypeAppService : IdentityAppServiceBase, IIdentityClaimTypeAppService
{
    /// <summary>
    /// I身份声明类型仓储
    /// </summary>
    protected IIdentityClaimTypeRepository ClaimTypeRepository { get; }
    /// <summary>
    /// 身份声明类型管理器
    /// </summary>
    protected IdentityClaimTypeManager ClaimTypeManager { get; }

    public IdentityClaimTypeAppService(
        IIdentityClaimTypeRepository claimTypeRepository,
        IdentityClaimTypeManager claimTypeManager)
    {
        ClaimTypeRepository = claimTypeRepository;
        ClaimTypeManager = claimTypeManager;
    }

    /// <summary>
    /// Task<Identity声明类型Dto>
    /// </summary>
    public virtual async Task<IdentityClaimTypeDto> GetAsync(Guid id)
    {
        return MapToDto(await ClaimTypeRepository.GetWithOptionsAsync(id));
    }

    /// <summary>
    /// Task<List结果Dto<Identity声明类型Dto>>
    /// </summary>
    public virtual async Task<ListResultDto<IdentityClaimTypeDto>> GetAllListAsync()
    {
        var list = await ClaimTypeRepository.GetListAsync(
            nameof(IdentityClaimType.Name),
            int.MaxValue,
            0,
            string.Empty);

        return new ListResultDto<IdentityClaimTypeDto>(MapToDtoList(list));
    }

    /// <summary>
    /// Task<Paged结果Dto<Identity声明类型Dto>>
    /// </summary>
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
    /// <summary>
    /// Task<Identity声明类型Dto>
    /// </summary>
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
        claimType.SetOptions(CreateOptions(claimType.Id, input.Options));

        claimType = await ClaimTypeManager.CreateAsync(claimType);

        return MapToDto(claimType);
    }

    [Authorize(IdentityPermissions.ClaimTypes.Update)]
    /// <summary>
    /// Task<Identity声明类型Dto>
    /// </summary>
    public virtual async Task<IdentityClaimTypeDto> UpdateAsync(Guid id, IdentityClaimTypeUpdateDto input)
    {
        var claimType = await ClaimTypeRepository.GetWithOptionsAsync(id);
        claimType.SetName(input.Name);
        claimType.Required = input.Required;
        claimType.Regex = input.Regex;
        claimType.RegexDescription = input.RegexDescription;
        claimType.Description = input.Description;
        claimType.ValueType = ParseValueType(input.ValueType);
        claimType.SetOptions(CreateOptions(claimType.Id, input.Options));

        claimType = await ClaimTypeManager.UpdateAsync(claimType);

        return MapToDto(claimType);
    }

    [Authorize(IdentityPermissions.ClaimTypes.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        await ClaimTypeManager.DeleteAsync(id);
    }

    /// <summary>
    /// 身份声明类型数据传输对象
    /// </summary>
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
            ValueType = claimType.ValueType.ToString(),
            Options = claimType.Options
                .OrderBy(x => x.Sort)
                .Select(x => new IdentityClaimTypeOptionDto
                {
                    Id = x.Id,
                    Label = x.Label,
                    Value = x.Value,
                    Sort = x.Sort,
                    IsEnabled = x.IsEnabled
                })
                .ToList()
        };
    }

    /// <summary>
    /// List<Identity声明类型Dto>
    /// </summary>
    protected virtual List<IdentityClaimTypeDto> MapToDtoList(List<IdentityClaimType> claimTypes)
    {
        var list = new List<IdentityClaimTypeDto>();
        foreach (var claimType in claimTypes)
        {
            list.Add(MapToDto(claimType));
        }

        return list;
    }

    /// <summary>
    /// 身份声明值类型
    /// </summary>
    protected virtual IdentityClaimValueType ParseValueType(string valueType)
    {
        return Enum.Parse<IdentityClaimValueType>(valueType, ignoreCase: true);
    }

    protected virtual List<IdentityClaimTypeOption> CreateOptions(
        Guid claimTypeId,
        IEnumerable<IdentityClaimTypeOptionCreateOrUpdateDto>? options)
    {
        return (options ?? Array.Empty<IdentityClaimTypeOptionCreateOrUpdateDto>())
            .Where(x => !x.Label.IsNullOrWhiteSpace() && !x.Value.IsNullOrWhiteSpace())
            .Select((x, index) => new IdentityClaimTypeOption(
                GuidGenerator.Create(),
                claimTypeId,
                x.Label,
                x.Value,
                x.Sort == 0 ? index + 1 : x.Sort,
                x.IsEnabled))
            .ToList();
    }
}
