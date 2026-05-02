using System;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace Censeq.Identity;

/// <summary>
/// 登录日志 API
/// </summary>
[RemoteService(Name = IdentityRemoteServiceConsts.RemoteServiceName)]
[Area(IdentityRemoteServiceConsts.ModuleName)]
[ControllerName("SecurityLog")]
[Route("api/identity/security-logs")]
public class IdentitySecurityLogController : AbpControllerBase, IIdentitySecurityLogAppService
{
    protected IIdentitySecurityLogAppService SecurityLogAppService { get; }

    public IdentitySecurityLogController(IIdentitySecurityLogAppService securityLogAppService)
    {
        SecurityLogAppService = securityLogAppService;
    }

    [HttpGet]
    public virtual Task<PagedResultDto<IdentitySecurityLogDto>> GetListAsync([FromQuery] GetSecurityLogsInput input)
    {
        return SecurityLogAppService.GetListAsync(input);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return SecurityLogAppService.DeleteAsync(id);
    }
}
