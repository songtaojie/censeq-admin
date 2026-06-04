using Censeq.Account.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Settings;

namespace Censeq.Account.Web.Settings
{
    /// <summary>
    /// 账户 Web 设置定义提供者。
    /// </summary>
    public class CenseqAccountSettingDefinitionProvider : SettingDefinitionProvider
    {
        /// <summary>
        /// 定义账户模块配置项。
        /// </summary>
        /// <param name="context">当前上下文。</param>
        public override void Define(ISettingDefinitionContext context)
        {
            context.Add(
                new SettingDefinition(
                    CenseqAccountSettingNames.EnableRememberMe,
                    "true",
                    L("DisplayName:Censeq.Account.EnableRememberMe"),
                    L("Description:Censeq.Account.EnableRememberMe"), isVisibleToClients: true)
            );

            context.Add(
                new SettingDefinition(
                    CenseqAccountSettingNames.EnableCaptcha,
                    "true",
                    L("DisplayName:Censeq.Account.EnableCaptcha"),
                    L("Description:Censeq.Account.EnableCaptcha"), isVisibleToClients: true)
            );
        }

        /// <summary>
        /// 创建账户模块的本地化字符串。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <returns>本地化字符串。</returns>
        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<AccountResource>(name);
        }
    }
}
