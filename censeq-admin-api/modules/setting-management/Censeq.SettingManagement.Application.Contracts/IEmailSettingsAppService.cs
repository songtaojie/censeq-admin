using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Censeq.SettingManagement;

public interface IEmailSettingsAppService : IApplicationService
{
    Task<EmailSettingsDto> GetAsync();

    Task UpdateAsync(UpdateEmailSettingsDto input);

    Task SendTestEmailAsync(SendTestEmailInput input);
}
