using Volo.Abp.Application.Dtos;

namespace Censeq.FileManagement.Files;

/// <summary>
/// 文件存储提供器配置应用服务契约。
/// </summary>
public interface IFileProviderAppService
{
    /// <summary>
    /// 分页查询文件存储提供器配置。
    /// </summary>
    Task<PagedResultDto<FileProviderDto>> GetListAsync(GetFileProvidersInput input);

    /// <summary>
    /// 获取指定文件存储提供器配置。
    /// </summary>
    Task<FileProviderDto> GetAsync(Guid id);

    /// <summary>
    /// 创建文件存储提供器配置。
    /// </summary>
    Task<FileProviderDto> CreateAsync(CreateUpdateFileProviderDto input);

    /// <summary>
    /// 更新文件存储提供器配置。
    /// </summary>
    Task<FileProviderDto> UpdateAsync(Guid id, CreateUpdateFileProviderDto input);

    /// <summary>
    /// 删除文件存储提供器配置。
    /// </summary>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// 将指定文件存储提供器设置为默认配置。
    /// </summary>
    Task SetDefaultAsync(Guid id);
}
