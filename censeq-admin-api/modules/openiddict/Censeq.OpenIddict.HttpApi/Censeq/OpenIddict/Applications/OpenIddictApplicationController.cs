using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Censeq.OpenIddict.Applications;

/// <summary>
/// OpenIddict 应用程序控制器，提供对应的 HTTP API。
/// </summary>
[RemoteService(Name = OpenIddictRemoteServiceConsts.RemoteServiceName)]
[Area(OpenIddictRemoteServiceConsts.ModuleName)]
[Route("api/openIddict/applications")]
public class OpenIddictApplicationController : OpenIddictControllerBase, IOpenIddictApplicationAppService
{
    /// <summary>
    /// OpenIddict 应用程序应用服务。
    /// </summary>
    protected IOpenIddictApplicationAppService ApplicationAppService { get; }

    /// <summary>
    /// 初始化 OpenIddictApplicationController 实例。
    /// </summary>
    /// <param name="applicationAppService">应用程序应用服务。</param>
    public OpenIddictApplicationController(IOpenIddictApplicationAppService applicationAppService)
    {
        ApplicationAppService = applicationAppService;
    }

    /// <summary>
    /// 获取分页数据列表。
    /// </summary>
    /// <param name="input">输入参数。</param>
    /// <returns>分页查询结果。</returns>
    [HttpGet]
    public virtual Task<PagedResultDto<OpenIddictApplicationDto>> GetListAsync(GetOpenIddictApplicationsInput input)
    {
        return ApplicationAppService.GetListAsync(input);
    }

    /// <summary>
    /// 获取指定标识的数据。
    /// </summary>
    /// <param name="id">标识。</param>
    /// <returns>查询结果。</returns>
    [HttpGet("{id}")]
    public virtual Task<OpenIddictApplicationDto> GetAsync(Guid id)
    {
        return ApplicationAppService.GetAsync(id);
    }

    /// <summary>
    /// 创建数据。
    /// </summary>
    /// <param name="input">输入参数。</param>
    /// <returns>创建后的数据。</returns>
    [HttpPost]
    public virtual Task<OpenIddictApplicationDto> CreateAsync(OpenIddictApplicationCreateDto input)
    {
        return ApplicationAppService.CreateAsync(input);
    }

    /// <summary>
    /// 更新数据。
    /// </summary>
    /// <param name="id">标识。</param>
    /// <param name="input">输入参数。</param>
    /// <returns>更新后的数据。</returns>
    [HttpPut("{id}")]
    public virtual Task<OpenIddictApplicationDto> UpdateAsync(Guid id, OpenIddictApplicationUpdateDto input)
    {
        return ApplicationAppService.UpdateAsync(id, input);
    }

    /// <summary>
    /// 删除数据。
    /// </summary>
    /// <param name="id">标识。</param>
    /// <returns>表示异步操作的任务。</returns>
    [HttpDelete("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return ApplicationAppService.DeleteAsync(id);
    }

    /// <summary>
    /// 生成新的客户端密钥。
    /// </summary>
    /// <param name="id">标识。</param>
    /// <returns>新生成的客户端密钥。</returns>
    [HttpPost("{id}/generate-secret")]
    public virtual Task<string> GenerateClientSecretAsync(Guid id)
    {
        return ApplicationAppService.GenerateClientSecretAsync(id);
    }

    /// <summary>
    /// 检查客户端标识是否已存在。
    /// </summary>
    /// <param name="clientId">客户端标识。</param>
    /// <param name="excludeId">需要排除的标识。</param>
    /// <returns>存在时返回 true，否则返回 false。</returns>
    [HttpGet("check-client-id")]
    public virtual Task<bool> CheckClientIdExistsAsync(string clientId, Guid? excludeId = null)
    {
        return ApplicationAppService.CheckClientIdExistsAsync(clientId, excludeId);
    }
}
