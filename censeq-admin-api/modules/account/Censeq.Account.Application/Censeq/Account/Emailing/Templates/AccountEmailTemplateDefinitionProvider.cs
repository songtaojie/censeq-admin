using Volo.Abp.Emailing.Templates;
using Volo.Abp.Localization;
using Volo.Abp.TextTemplating;
using Censeq.Account.Localization;

namespace Censeq.Account.Emailing.Templates;

/// <summary>
/// 账户邮件模板定义提供者。
/// </summary>
public class AccountEmailTemplateDefinitionProvider : TemplateDefinitionProvider
{
    /// <summary>
    /// 定义账户模块配置项。
    /// </summary>
    /// <param name="context">当前上下文。</param>
    public override void Define(ITemplateDefinitionContext context)
    {
        context.Add(
            new TemplateDefinition(
                AccountEmailTemplates.PasswordResetLink,
                displayName: LocalizableString.Create<AccountResource>($"TextTemplate:{AccountEmailTemplates.PasswordResetLink}"),
                layout: StandardEmailTemplates.Layout,
                localizationResource: typeof(AccountResource)
            ).WithVirtualFilePath("/Censeq/Account/Emailing/Templates/PasswordResetLink.tpl", true)
        );
    }
}
