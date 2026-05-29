using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;

namespace Censeq.FileManagement.Files;

/// <summary>
/// 文件上传、下载和头像更新应用服务契约。
/// </summary>
public interface IFileUploadService
{
    /// <summary>
    /// 分页查询文件上传记录。
    /// </summary>
    Task<PagedResultDto<FileRecordDto>> GetListAsync(GetFileRecordsInput input);

    /// <summary>
    /// 上传通用文件并保存文件记录。
    /// </summary>
    Task<FileRecordDto> UploadAsync(IFormFile? file, string? category, bool isPublic, bool allowImageOnly);

    /// <summary>
    /// 上传当前用户头像并更新用户扩展属性 AvatarUrl。
    /// </summary>
    Task<FileRecordDto> UploadAvatarAsync(IFormFile? file);

    /// <summary>
    /// 下载指定文件。
    /// </summary>
    Task<FileStreamResult> DownloadAsync(Guid id);

    /// <summary>
    /// 删除文件记录及对应的物理文件。
    /// </summary>
    Task DeleteAsync(Guid id);
}
