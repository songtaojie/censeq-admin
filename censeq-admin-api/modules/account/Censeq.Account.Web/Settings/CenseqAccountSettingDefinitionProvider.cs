using Volo.Abp.Account.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Settings;

namespace Censeq.Account.Web.Settings
{
    public class CenseqAccountSettingDefinitionProvider : SettingDefinitionProvider
    {
        public override void Define(ISettingDefinitionContext context)
        {
            context.Add(
                new SettingDefinition(
                    CenseqAccountSettingNames.EnableRememberMe,
                    "true",
                    L("DisplayName:Censeq.Account.EnableRememberMe"),
                    L("Description:Censeq.Account.EnableRememberMe"), isVisibleToClients: true)
            );
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<AccountResource>(name);
        }
    }
}
