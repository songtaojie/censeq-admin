using Volo.Abp.Application.Dtos;

namespace Censeq.FileManagement.Files;

public interface IFileProviderAppService
{
    Task<PagedResultDto<FileProviderDto>> GetListAsync(GetFileProvidersInput input);

    Task<FileProviderDto> GetAsync(Guid id);

    Task<FileProviderDto> CreateAsync(CreateUpdateFileProviderDto input);

    Task<FileProviderDto> UpdateAsync(Guid id, CreateUpdateFileProviderDto input);

    Task DeleteAsync(Guid id);

    Task SetDefaultAsync(Guid id);
}
