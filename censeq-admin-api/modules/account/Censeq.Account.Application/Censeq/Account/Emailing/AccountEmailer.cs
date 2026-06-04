using System;
using System.Diagnostics;
using System.Net;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Emailing;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TextTemplating;
using Volo.Abp.UI.Navigation.Urls;
using Censeq.Account.Emailing.Templates;
using Censeq.Account.Localization;
using Censeq.Identity.Entities;

namespace Censeq.Account.Emailing;

/// <summary>
/// 账户邮件发送器，负责发送账户相关邮件。
/// </summary>
public class AccountEmailer : IAccountEmailer, ITransientDependency
{
    /// <summary>
    /// 模板渲染器。
    /// </summary>
    protected ITemplateRenderer TemplateRenderer { get; }
    /// <summary>
    /// 邮件发送器。
    /// </summary>
    protected IEmailSender EmailSender { get; }
    /// <summary>
    /// 字符串本地化器。
    /// </summary>
    protected IStringLocalizer<AccountResource> StringLocalizer { get; }
    /// <summary>
    /// 应用地址提供者。
    /// </summary>
    protected IAppUrlProvider AppUrlProvider { get; }
    /// <summary>
    /// 当前租户。
    /// </summary>
    protected ICurrentTenant CurrentTenant { get; }

    /// <summary>
    /// 初始化 AccountEmailer 实例。
    /// </summary>
    /// <param name="emailSender">邮件发送器。</param>
    /// <param name="templateRenderer">模板渲染器。</param>
    /// <param name="stringLocalizer">字符串本地化器。</param>
    /// <param name="appUrlProvider">应用地址提供者。</param>
    /// <param name="currentTenant">当前租户。</param>
    public AccountEmailer(
        IEmailSender emailSender,
        ITemplateRenderer templateRenderer,
        IStringLocalizer<AccountResource> stringLocalizer,
        IAppUrlProvider appUrlProvider,
        ICurrentTenant currentTenant)
    {
        EmailSender = emailSender;
        StringLocalizer = stringLocalizer;
        AppUrlProvider = appUrlProvider;
        CurrentTenant = currentTenant;
        TemplateRenderer = templateRenderer;
    }

    /// <summary>
    /// 异步发送密码重置链接。
    /// </summary>
    /// <param name="user">用户。</param>
    /// <param name="resetToken">重置令牌。</param>
    /// <param name="appName">应用程序名称。</param>
    /// <param name="returnUrl">返回地址。</param>
    /// <param name="returnUrlHash">返回地址哈希。</param>
    /// <returns>表示异步操作的任务。</returns>
    public virtual async Task SendPasswordResetLinkAsync(
        IdentityUser user,
        string resetToken,
        string appName,
        string? returnUrl = null,
        string? returnUrlHash = null)
    {
        Debug.Assert(CurrentTenant.Id == user.TenantId, "This method can only work for current tenant!");

        var url = await AppUrlProvider.GetResetPasswordUrlAsync(appName);

        var link = $"{url}?userId={user.Id}&{TenantResolverConsts.DefaultTenantKey}={user.TenantId}&resetToken={UrlEncoder.Default.Encode(resetToken)}";

        if (!returnUrl.IsNullOrEmpty())
        {
            link += "&returnUrl=" + NormalizeReturnUrl(returnUrl);
        }

        if (!returnUrlHash.IsNullOrEmpty())
        {
            link += "&returnUrlHash=" + returnUrlHash;
        }

        var emailContent = await TemplateRenderer.RenderAsync(
            AccountEmailTemplates.PasswordResetLink,
            new { link = link }
        );

        await EmailSender.SendAsync(
            user.Email!,
            StringLocalizer["PasswordReset"],
            emailContent
        );
    }

    /// <summary>
    /// 规范化返回地址。
    /// </summary>
    /// <param name="returnUrl">返回地址。</param>
    /// <returns>规范化后的返回地址。</returns>
    protected virtual string NormalizeReturnUrl(string returnUrl)
    {
        if (returnUrl.IsNullOrEmpty())
        {
            return returnUrl;
        }

        if (returnUrl.StartsWith("/connect/authorize/callback", StringComparison.OrdinalIgnoreCase))
        {
            if (returnUrl.Contains("?"))
            {
                var queryPart = returnUrl.Split('?')[1];
                var queryParameters = queryPart.Split('&');
                foreach (var queryParameter in queryParameters)
                {
                    if (queryParameter.Contains("="))
                    {
                        var queryParam = queryParameter.Split('=');
                        if (queryParam[0] == "redirect_uri")
                        {
                            return WebUtility.UrlDecode(queryParam[1]);
                        }
                    }
                }
            }
        }

        if (returnUrl.StartsWith("/connect/authorize?", StringComparison.OrdinalIgnoreCase))
        {
            return WebUtility.UrlEncode(returnUrl);
        }

        return returnUrl;
    }
}
