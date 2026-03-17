using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Censeq.Identity;

namespace Censeq.Account;

public interface IAccountAppService : IApplicationService
{
    Task<IdentityUserDto> RegisterAsync(RegisterDto input);

    Task SendPasswordResetCodeAsync(SendPasswordResetCodeDto input);

    Task<bool> VerifyPasswordResetTokenAsync(VerifyPasswordResetTokenInput input);

    Task ResetPasswordAsync(ResetPasswordDto input);
}
