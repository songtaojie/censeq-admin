using Censeq.Admin.SystemMonitor;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Censeq.Admin.Controllers;

[RemoteService(Name = "Admin")]
[Area("admin")]
[Route("api/admin/system-monitor")]
public class SystemMonitorController : AdminController, ISystemMonitorAppService
{
    private readonly ISystemMonitorAppService _appService;

    public SystemMonitorController(ISystemMonitorAppService appService)
    {
        _appService = appService;
    }

    [HttpGet("server/base")]
    public Task<SystemBaseInfoDto> GetServerBaseAsync()
    {
        return _appService.GetServerBaseAsync();
    }

    [HttpGet("server/usage")]
    public Task<SystemUsageInfoDto> GetServerUsageAsync()
    {
        return _appService.GetServerUsageAsync();
    }

    [HttpGet("server/disks")]
    public Task<ListResultDto<SystemDiskInfoDto>> GetServerDisksAsync()
    {
        return _appService.GetServerDisksAsync();
    }

    [HttpGet("server/assemblies")]
    public Task<ListResultDto<AssemblyInfoDto>> GetAssemblyListAsync()
    {
        return _appService.GetAssemblyListAsync();
    }

    [HttpGet("cache/keys")]
    public Task<ListResultDto<string>> GetCacheKeysAsync()
    {
        return _appService.GetCacheKeysAsync();
    }

    [HttpGet("cache/value/{key}")]
    public Task<object?> GetCacheValueAsync(string key)
    {
        return _appService.GetCacheValueAsync(key);
    }

    [HttpDelete("cache/{key}")]
    public Task DeleteCacheAsync(string key)
    {
        return _appService.DeleteCacheAsync(key);
    }

    [HttpDelete("cache")]
    public Task ClearCacheAsync()
    {
        return _appService.ClearCacheAsync();
    }
}
