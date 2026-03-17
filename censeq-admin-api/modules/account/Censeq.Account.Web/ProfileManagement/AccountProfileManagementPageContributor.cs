using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Censeq.Account.Localization;
using Censeq.Account.Web.Pages.Account.Components.ProfileManagementGroup.Password;
using Censeq.Account.Web.Pages.Account.Components.ProfileManagementGroup.PersonalInfo;
using Censeq.Identity;
using Volo.Abp.Users;

namespace Censeq.Account.Web.ProfileManagement;

public class AccountProfileManagementPageContributor : IProfileManagementPageContributor
{
    public async Task ConfigureAsync(ProfileManagementPageCreationContext context)
    {
        var l = context.ServiceProvider.GetRequiredService<IStringLocalizer<AccountResource>>();

        if (await IsPasswordChangeEnabled(context))
        {
            context.Groups.Add(
                new ProfileManagementPageGroup(
                    "Censeq.Account.Password",
                    l["ProfileTab:Password"],
                    typeof(AccountProfilePasswordManagementGroupViewComponent)
                )
            );
        }

        context.Groups.Add(
            new ProfileManagementPageGroup(
                "Censeq.Account.PersonalInfo",
                l["ProfileTab:PersonalInfo"],
                typeof(AccountProfilePersonalInfoManagementGroupViewComponent)
            )
        );
    }

    protected virtual async Task<bool> IsPasswordChangeEnabled(ProfileManagementPageCreationContext context)
    {
        var userManager = context.ServiceProvider.GetRequiredService<IdentityUserManager>();
        var currentUser = context.ServiceProvider.GetRequiredService<ICurrentUser>();

        var user = await userManager.GetByIdAsync(currentUser.GetId());

        return !user.IsExternal;
    }
}
