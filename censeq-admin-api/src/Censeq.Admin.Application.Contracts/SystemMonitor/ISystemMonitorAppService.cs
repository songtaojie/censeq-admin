using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Censeq.Admin.SystemMonitor;

public interface ISystemMonitorAppService : IApplicationService
{
    Task<SystemBaseInfoDto> GetServerBaseAsync();

    Task<SystemUsageInfoDto> GetServerUsageAsync();

    Task<ListResultDto<SystemDiskInfoDto>> GetServerDisksAsync();

    Task<ListResultDto<AssemblyInfoDto>> GetAssemblyListAsync();

    Task<ListResultDto<string>> GetCacheKeysAsync();

    Task<object?> GetCacheValueAsync(string key);

    Task DeleteCacheAsync(string key);

    Task ClearCacheAsync();
}
