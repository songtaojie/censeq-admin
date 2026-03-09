using Censeq.Admin.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Settings;

namespace Censeq.Admin.Settings;

public class AdminSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        context.Add(
               new SettingDefinition(
                   AdminSettingNames.IsSelfRegistrationEnabled,
                   "true",
                   L("DisplayName:Abp.Account.IsSelfRegistrationEnabled"),
                   L("Description:Abp.Account.IsSelfRegistrationEnabled"), isVisibleToClients: true)
           );

        context.Add(
            new SettingDefinition(
                AdminSettingNames.EnableLocalLogin,
                "true",
                L("DisplayName:Abp.Account.EnableLocalLogin"),
                L("Description:Abp.Account.EnableLocalLogin"), isVisibleToClients: true)
        );
        context.Add(
            new SettingDefinition(
                AdminSettingNames.EnableRememberMe,
                "true",
                L("DisplayName:Abp.Account.EnableLocalLogin"),
                L("Description:Abp.Account.EnableLocalLogin"), isVisibleToClients: true)
        );
        context.Add(
            new SettingDefinition(
                AdminSettingNames.ShowCancelButton,
                "true",
                L("DisplayName:Abp.Account.EnableLocalLogin"),
                L("Description:Abp.Account.EnableLocalLogin"), isVisibleToClients: true)
        );

    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<CenseqAdminResource>(name);
    }
}
