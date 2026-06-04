using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Censeq.Account.Localization;
using Censeq.Account.Web.Pages.Account.Components.ProfileManagementGroup.Password;
using Censeq.Account.Web.Pages.Account.Components.ProfileManagementGroup.PersonalInfo;
using Censeq.Identity;
using Volo.Abp.Users;

namespace Censeq.Account.Web.ProfileManagement;

/// <summary>
/// 账户个人资料管理页面贡献器。
/// </summary>
public class AccountProfileManagementPageContributor : IProfileManagementPageContributor
{
    /// <summary>
    /// 异步配置页面分组。
    /// </summary>
    /// <param name="context">当前上下文。</param>
    /// <returns>表示异步操作的任务。</returns>
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

    /// <summary>
    /// Is 密码 修改 Enabled。
    /// </summary>
    /// <param name="context">当前上下文。</param>
    /// <returns>异步操作结果。</returns>
    protected virtual async Task<bool> IsPasswordChangeEnabled(ProfileManagementPageCreationContext context)
    {
        var userManager = context.ServiceProvider.GetRequiredService<IdentityUserManager>();
        var currentUser = context.ServiceProvider.GetRequiredService<ICurrentUser>();

        var user = await userManager.GetByIdAsync(currentUser.GetId());

        return !user.IsExternal;
    }
}
