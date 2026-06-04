using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenIddict.Abstractions;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Censeq.OpenIddict.Scopes;

/// <summary>
/// OpenIddict 作用域应用服务，提供应用层操作。
/// </summary>
public class OpenIddictScopeAppService :
    CrudAppService<
        OpenIddictScope,
        OpenIddictScopeDto,
        Guid,
        GetOpenIddictScopesInput,
        OpenIddictScopeCreateDto,
        OpenIddictScopeUpdateDto>,
    IOpenIddictScopeAppService
{
    /// <summary>
    /// OpenIddict 作用域管理器。
    /// </summary>
    protected IOpenIddictScopeManager ScopeManager { get; }
    /// <summary>
    /// OpenIddict 作用域仓储。
    /// </summary>
    protected IOpenIddictScopeRepository ScopeRepository { get; }

    /// <summary>
    /// 初始化 OpenIddictScopeAppService 实例。
    /// </summary>
    /// <param name="repository">仓储。</param>
    /// <param name="scopeManager">OpenIddict 作用域管理器。</param>
    public OpenIddictScopeAppService(
        IOpenIddictScopeRepository repository,
        IOpenIddictScopeManager scopeManager)
        : base(repository)
    {
        ScopeManager = scopeManager;
        ScopeRepository = repository;

        CreatePolicyName = OpenIddictPermissions.Scopes.Create;
        UpdatePolicyName = OpenIddictPermissions.Scopes.Update;
        DeletePolicyName = OpenIddictPermissions.Scopes.Delete;
        GetPolicyName = OpenIddictPermissions.Scopes.Default;
        GetListPolicyName = OpenIddictPermissions.Scopes.Default;
    }

    /// <summary>
    /// 获取指定标识的数据。
    /// </summary>
    /// <param name="id">标识。</param>
    /// <returns>查询结果。</returns>
    public override async Task<OpenIddictScopeDto> GetAsync(Guid id)
    {
        var scope = await ScopeRepository.GetAsync(id);
        return MapToDto(scope);
    }

    /// <summary>
    /// 获取分页数据列表。
    /// </summary>
    /// <param name="input">输入参数。</param>
    /// <returns>分页查询结果。</returns>
    public override async Task<PagedResultDto<OpenIddictScopeDto>> GetListAsync(GetOpenIddictScopesInput input)
    {
        await CheckGetListPolicyAsync();

        var count = await ScopeRepository.GetCountAsync(input.Filter);

        var list = await ScopeRepository.GetListAsync(
            input.Sorting,
            input.SkipCount,
            input.MaxResultCount,
            input.Filter);

        var dtos = list.Select(MapToDto).ToList();

        return new PagedResultDto<OpenIddictScopeDto>(count, dtos);
    }

    /// <summary>
    /// 创建数据。
    /// </summary>
    /// <param name="input">输入参数。</param>
    /// <returns>创建后的数据。</returns>
    public override async Task<OpenIddictScopeDto> CreateAsync(OpenIddictScopeCreateDto input)
    {
        await CheckCreatePolicyAsync();

        // 检查名称是否已存在
        var existing = await ScopeManager.FindByNameAsync(input.Name);
        if (existing != null)
        {
            throw new UserFriendlyException($"作用域名称 '{input.Name}' 已存在");
        }

        var descriptor = new OpenIddictScopeDescriptor
        {
            Name = input.Name,
            DisplayName = input.DisplayName,
            Description = input.Description
        };

        foreach (var resource in input.Resources.Where(r => !string.IsNullOrWhiteSpace(r)))
        {
            descriptor.Resources.Add(resource);
        }

        await ScopeManager.CreateAsync(descriptor);
        var scope = await ScopeRepository.FindByNameAsync(input.Name);

        return MapToDto(scope);
    }

    /// <summary>
    /// 更新数据。
    /// </summary>
    /// <param name="id">标识。</param>
    /// <param name="input">输入参数。</param>
    /// <returns>更新后的数据。</returns>
    public override async Task<OpenIddictScopeDto> UpdateAsync(Guid id, OpenIddictScopeUpdateDto input)
    {
        await CheckUpdatePolicyAsync();

        var scope = await ScopeRepository.GetAsync(id);
        var scopeModel = scope.ToModel();

        var descriptor = new OpenIddictScopeDescriptor();
        await ScopeManager.PopulateAsync(descriptor, scopeModel);

        descriptor.DisplayName = input.DisplayName;
        descriptor.Description = input.Description;

        descriptor.Resources.Clear();
        foreach (var resource in input.Resources.Where(r => !string.IsNullOrWhiteSpace(r)))
        {
            descriptor.Resources.Add(resource);
        }

        await ScopeManager.UpdateAsync(scopeModel, descriptor);
        scope = await ScopeRepository.GetAsync(id);

        return MapToDto(scope);
    }

    /// <summary>
    /// 删除数据。
    /// </summary>
    /// <param name="id">标识。</param>
    /// <returns>表示异步操作的任务。</returns>
    public override async Task DeleteAsync(Guid id)
    {
        await CheckDeletePolicyAsync();

        var scope = await ScopeRepository.GetAsync(id);
        await ScopeManager.DeleteAsync(scope.ToModel());
    }

    /// <summary>
    /// 检查名称是否已存在。
    /// </summary>
    /// <param name="name">name。</param>
    /// <param name="excludeId">需要排除的标识。</param>
    /// <returns>存在时返回 true，否则返回 false。</returns>
    public virtual async Task<bool> CheckNameExistsAsync(string name, Guid? excludeId = null)
    {
        var existing = await ScopeRepository.FindByNameAsync(name);
        if (existing == null) return false;

        if (excludeId.HasValue && existing.Id == excludeId.Value) return false;

        return true;
    }

    /// <summary>
    /// 将领域对象映射为 DTO。
    /// </summary>
    /// <param name="scope">OpenIddict 作用域。</param>
    /// <returns>操作结果。</returns>
    protected virtual OpenIddictScopeDto MapToDto(OpenIddictScope scope)
    {
        return new OpenIddictScopeDto
        {
            Id = scope.Id,
            Name = scope.Name,
            DisplayName = scope.DisplayName,
            Description = scope.Description,
            Resources = string.IsNullOrEmpty(scope.Resources)
                ? new List<string>()
                : System.Text.Json.JsonSerializer.Deserialize<List<string>>(scope.Resources) ?? new List<string>(),
            CreationTime = scope.CreationTime,
            CreatorId = scope.CreatorId,
            LastModificationTime = scope.LastModificationTime,
            LastModifierId = scope.LastModifierId
        };
    }
}
