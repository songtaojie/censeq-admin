using Censeq.FileManagement.Files;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace Censeq.FileManagement.Controllers;

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

    [HttpGet]
    public virtual Task<PagedResultDto<FileProviderDto>> GetListAsync([FromQuery] GetFileProvidersInput input)
    {
        return _fileProviderAppService.GetListAsync(input);
    }

    [HttpGet("{id:guid}")]
    public virtual Task<FileProviderDto> GetAsync(Guid id)
    {
        return _fileProviderAppService.GetAsync(id);
    }

    [HttpPost]
    public virtual Task<FileProviderDto> CreateAsync(CreateUpdateFileProviderDto input)
    {
        return _fileProviderAppService.CreateAsync(input);
    }

    [HttpPut("{id:guid}")]
    public virtual Task<FileProviderDto> UpdateAsync(Guid id, CreateUpdateFileProviderDto input)
    {
        return _fileProviderAppService.UpdateAsync(id, input);
    }

    [HttpDelete("{id:guid}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return _fileProviderAppService.DeleteAsync(id);
    }

    [HttpPost("{id:guid}/default")]
    public virtual Task SetDefaultAsync(Guid id)
    {
        return _fileProviderAppService.SetDefaultAsync(id);
    }
}
