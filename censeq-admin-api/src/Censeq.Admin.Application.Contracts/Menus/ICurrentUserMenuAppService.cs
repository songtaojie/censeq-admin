using Volo.Abp.Application.Services;

namespace Censeq.Admin.Menus;

public interface ICurrentUserMenuAppService : IApplicationService
{
    Task<CurrentUserMenuResultDto> GetAsync();
}