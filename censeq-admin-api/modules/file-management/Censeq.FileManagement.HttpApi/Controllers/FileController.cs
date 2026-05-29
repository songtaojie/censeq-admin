using Censeq.FileManagement.Files;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace Censeq.FileManagement.Controllers;

/// <summary>
/// 文件上传、下载和头像维护接口。
/// </summary>
[Authorize]
[Area("Admin")]
[Route("api/admin/files")]
public class FileController : AbpControllerBase
{
    private readonly IFileUploadService _fileUploadService;

    public FileController(IFileUploadService fileUploadService)
    {
        _fileUploadService = fileUploadService;
    }

    /// <summary>
    /// 分页获取文件上传记录。
    /// </summary>
    [HttpGet]
    public virtual async Task<PagedResultDto<FileRecordDto>> GetListAsync([FromQuery] GetFileRecordsInput input)
    {
        return await _fileUploadService.GetListAsync(input);
    }

    /// <summary>
    /// 上传通用文件，并写入文件记录。
    /// </summary>
    [HttpPost]
    [Consumes("multipart/form-data")]
    public virtual async Task<FileRecordDto> UploadAsync([FromForm] UploadFileRequest input)
    {
        return await _fileUploadService.UploadAsync(input.File, input.Category, input.IsPublic, input.AllowImageOnly);
    }

    /// <summary>
    /// 上传当前用户头像，并同步更新用户扩展属性 AvatarUrl。
    /// </summary>
    [HttpPost("avatar")]
    [Consumes("multipart/form-data")]
    public virtual async Task<FileRecordDto> UploadAvatarAsync([FromForm] UploadAvatarRequest input)
    {
        return await _fileUploadService.UploadAvatarAsync(input.File);
    }

    /// <summary>
    /// 下载指定文件。
    /// </summary>
    [HttpGet("{id:guid}/download")]
    public virtual async Task<IActionResult> DownloadAsync(Guid id)
    {
        return await _fileUploadService.DownloadAsync(id);
    }

    /// <summary>
    /// 删除文件记录及其物理文件。
    /// </summary>
    [HttpDelete("{id:guid}")]
    public virtual async Task DeleteAsync(Guid id)
    {
        await _fileUploadService.DeleteAsync(id);
    }
}

/// <summary>
/// 通用文件上传请求。
/// </summary>
public class UploadFileRequest
{
    /// <summary>
    /// 待上传文件。
    /// </summary>
    public IFormFile? File { get; set; }

    /// <summary>
    /// 业务分类，未传时默认 common。
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// 是否作为公开文件保存。
    /// </summary>
    public bool IsPublic { get; set; }

    /// <summary>
    /// 是否限制为图片类型。
    /// </summary>
    public bool AllowImageOnly { get; set; }
}

/// <summary>
/// 头像上传请求。
/// </summary>
public class UploadAvatarRequest
{
    /// <summary>
    /// 头像文件。
    /// </summary>
    public IFormFile? File { get; set; }
}
