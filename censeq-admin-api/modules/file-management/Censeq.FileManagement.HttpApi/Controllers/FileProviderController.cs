using Censeq.FileManagement.Files;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace Censeq.FileManagement.Controllers;

/// <summary>
/// 文件存储提供器配置管理接口。
/// </summary>
[Authorize]
[Area("Admin")]
[Route("api/admin/file-providers")]
public class FileProviderController : AbpControllerBase
{
    private readonly IFileProviderAppService _fileProviderAppService;

    public FileProviderController(IFileProviderAppService fileProviderAppService)
    {
        _fileProviderAppService = fileProviderAppService;
    }

    /// <summary>
    /// 分页查询文件存储提供器配置。
    /// </summary>
    [HttpGet]
    public virtual Task<PagedResultDto<FileProviderDto>> GetListAsync([FromQuery] GetFileProvidersInput input)
    {
        return _fileProviderAppService.GetListAsync(input);
    }

    /// <summary>
    /// 获取指定文件存储提供器配置。
    /// </summary>
    [HttpGet("{id:guid}")]
    public virtual Task<FileProviderDto> GetAsync(Guid id)
    {
        return _fileProviderAppService.GetAsync(id);
    }

    /// <summary>
    /// 创建文件存储提供器配置。
    /// </summary>
    [HttpPost]
    public virtual Task<FileProviderDto> CreateAsync(CreateUpdateFileProviderDto input)
    {
        return _fileProviderAppService.CreateAsync(input);
    }

    /// <summary>
    /// 更新文件存储提供器配置。
    /// </summary>
    [HttpPut("{id:guid}")]
    public virtual Task<FileProviderDto> UpdateAsync(Guid id, CreateUpdateFileProviderDto input)
    {
        return _fileProviderAppService.UpdateAsync(id, input);
    }

    /// <summary>
    /// 删除文件存储提供器配置。
    /// </summary>
    [HttpDelete("{id:guid}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return _fileProviderAppService.DeleteAsync(id);
    }

    /// <summary>
    /// 将指定文件存储提供器设置为默认提供器。
    /// </summary>
    [HttpPost("{id:guid}/default")]
    public virtual Task SetDefaultAsync(Guid id)
    {
        return _fileProviderAppService.SetDefaultAsync(id);
    }
}
