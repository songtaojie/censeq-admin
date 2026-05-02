using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Censeq.Identity;

/// <summary>
/// 登录日志应用服务接口
/// </summary>
public interface IIdentitySecurityLogAppService : IApplicationService
{
    Task<PagedResultDto<IdentitySecurityLogDto>> GetListAsync(GetSecurityLogsInput input);

    Task DeleteAsync(Guid id);
}