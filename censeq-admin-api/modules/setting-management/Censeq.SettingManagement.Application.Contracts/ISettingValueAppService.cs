using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Censeq.SettingManagement;

public interface ISettingValueAppService : IApplicationService
{
    Task<SettingValueDto> GetAsync(string name);

    Task SetAsync(SetSettingValueInput input);

    Task DeleteAsync(string name, string providerName, string? providerKey);
}
