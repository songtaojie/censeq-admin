using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;

namespace Censeq.FileManagement.Files;

public interface IFileUploadService
{
    Task<PagedResultDto<FileRecordDto>> GetListAsync(GetFileRecordsInput input);

    Task<FileRecordDto> UploadAsync(IFormFile? file, string? category, bool isPublic, bool allowImageOnly);

    Task<FileRecordDto> UploadAvatarAsync(IFormFile? file);

    Task<FileStreamResult> DownloadAsync(Guid id);

    Task DeleteAsync(Guid id);
}
