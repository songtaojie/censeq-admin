using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Censeq.Identity.Entities;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;

namespace Censeq.Identity;

/// <summary>
/// 登录日志应用服务
/// </summary>
[Authorize(IdentityPermissions.SecurityLogs.Default)]
public class IdentitySecurityLogAppService : IdentityAppServiceBase, IIdentitySecurityLogAppService
{
    protected IIdentitySecurityLogRepository SecurityLogRepository { get; }

    public IdentitySecurityLogAppService(IIdentitySecurityLogRepository securityLogRepository)
    {
        SecurityLogRepository = securityLogRepository;
    }

    public virtual async Task<PagedResultDto<IdentitySecurityLogDto>> GetListAsync(GetSecurityLogsInput input)
    {
        var count = await SecurityLogRepository.GetCountAsync(
            startTime: input.StartTime,
            endTime: input.EndTime,
            applicationName: null,
            identity: null,
            action: input.Action,
            userId: null,
            userName: input.UserName,
            clientId: null,
            correlationId: null
        );

        var list = await SecurityLogRepository.GetListAsync(
            sorting: input.Sorting,
            maxResultCount: input.MaxResultCount,
            skipCount: input.SkipCount,
            startTime: input.StartTime,
            endTime: input.EndTime,
            applicationName: null,
            identity: null,
            action: input.Action,
            userId: null,
            userName: input.UserName,
            clientId: null,
            correlationId: null
        );

        return new PagedResultDto<IdentitySecurityLogDto>(
            count,
            ObjectMapper.Map<List<IdentitySecurityLog>, List<IdentitySecurityLogDto>>(list)
        );
    }

    [Authorize(IdentityPermissions.SecurityLogs.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        await SecurityLogRepository.DeleteAsync(id);
    }
}
