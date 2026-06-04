using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Censeq.OpenIddict.Scopes;

/// <summary>
/// OpenIddict 作用域控制器，提供对应的 HTTP API。
/// </summary>
[RemoteService(Name = OpenIddictRemoteServiceConsts.RemoteServiceName)]
[Area(OpenIddictRemoteServiceConsts.ModuleName)]
[Route("api/openIddict/scopes")]
public class OpenIddictScopeController : OpenIddictControllerBase, IOpenIddictScopeAppService
{
    /// <summary>
    /// OpenIddict 作用域应用服务。
    /// </summary>
    protected IOpenIddictScopeAppService ScopeAppService { get; }

    /// <summary>
    /// 初始化 OpenIddictScopeController 实例。
    /// </summary>
    /// <param name="scopeAppService">作用域应用服务。</param>
    public OpenIddictScopeController(IOpenIddictScopeAppService scopeAppService)
    {
        ScopeAppService = scopeAppService;
    }

    /// <summary>
    /// 获取分页数据列表。
    /// </summary>
    /// <param name="input">输入参数。</param>
    /// <returns>分页查询结果。</returns>
    [HttpGet]
    public virtual Task<PagedResultDto<OpenIddictScopeDto>> GetListAsync(GetOpenIddictScopesInput input)
    {
        return ScopeAppService.GetListAsync(input);
    }

    /// <summary>
    /// 获取指定标识的数据。
    /// </summary>
    /// <param name="id">标识。</param>
    /// <returns>查询结果。</returns>
    [HttpGet("{id}")]
    public virtual Task<OpenIddictScopeDto> GetAsync(Guid id)
    {
        return ScopeAppService.GetAsync(id);
    }

    /// <summary>
    /// 创建数据。
    /// </summary>
    /// <param name="input">输入参数。</param>
    /// <returns>创建后的数据。</returns>
    [HttpPost]
    public virtual Task<OpenIddictScopeDto> CreateAsync(OpenIddictScopeCreateDto input)
    {
        return ScopeAppService.CreateAsync(input);
    }

    /// <summary>
    /// 更新数据。
    /// </summary>
    /// <param name="id">标识。</param>
    /// <param name="input">输入参数。</param>
    /// <returns>更新后的数据。</returns>
    [HttpPut("{id}")]
    public virtual Task<OpenIddictScopeDto> UpdateAsync(Guid id, OpenIddictScopeUpdateDto input)
    {
        return ScopeAppService.UpdateAsync(id, input);
    }

    /// <summary>
    /// 删除数据。
    /// </summary>
    /// <param name="id">标识。</param>
    /// <returns>表示异步操作的任务。</returns>
    [HttpDelete("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return ScopeAppService.DeleteAsync(id);
    }

    /// <summary>
    /// 检查名称是否已存在。
    /// </summary>
    /// <param name="name">name。</param>
    /// <param name="excludeId">需要排除的标识。</param>
    /// <returns>存在时返回 true，否则返回 false。</returns>
    [HttpGet("check-name")]
    public virtual Task<bool> CheckNameExistsAsync(string name, Guid? excludeId = null)
    {
        return ScopeAppService.CheckNameExistsAsync(name, excludeId);
    }
}
