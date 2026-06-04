using Volo.Abp.Localization;
using Volo.Abp.Settings;
using Censeq.Account.Localization;

namespace Censeq.Account.Settings;

/// <summary>
/// 账户设置定义提供者。
/// </summary>
public class AccountSettingDefinitionProvider : SettingDefinitionProvider
{
    /// <summary>
    /// 定义账户模块配置项。
    /// </summary>
    /// <param name="context">当前上下文。</param>
    public override void Define(ISettingDefinitionContext context)
    {
        context.Add(
            new SettingDefinition(
                AccountSettingNames.IsSelfRegistrationEnabled,
                "true",
                L("DisplayName:Abp.Account.IsSelfRegistrationEnabled"),
                L("Description:Abp.Account.IsSelfRegistrationEnabled"), isVisibleToClients: true)
        );

        context.Add(
            new SettingDefinition(
                AccountSettingNames.EnableLocalLogin,
                "true",
                L("DisplayName:Abp.Account.EnableLocalLogin"),
                L("Description:Abp.Account.EnableLocalLogin"), isVisibleToClients: true)
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
