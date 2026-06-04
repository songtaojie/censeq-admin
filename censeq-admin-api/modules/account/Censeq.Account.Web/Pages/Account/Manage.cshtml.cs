using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Censeq.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Censeq.Account.Web.ProfileManagement;
using Volo.Abp.Validation;

namespace Censeq.Account.Web.Pages.Account;

/// <summary>
/// 账户管理页面模型。
/// </summary>
public class ManageModel : AccountPageModel
{
    /// <summary>
    /// 返回地址。
    /// </summary>
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    /// <summary>
    /// 个人资料管理页面创建上下文。
    /// </summary>
    public ProfileManagementPageCreationContext ProfileManagementPageCreationContext { get; private set; }

    /// <summary>
    /// 配置项。
    /// </summary>
    protected ProfileManagementPageOptions Options { get; }

    /// <summary>
    /// 初始化 ManageModel 实例。
    /// </summary>
    /// <param name="options">配置项。</param>
    public ManageModel(IOptions<ProfileManagementPageOptions> options)
    {
        Options = options.Value;
        ProfileManagementPageCreationContext = default!;
    }

    /// <summary>
    /// 异步处理页面 GET 请求。
    /// </summary>
    /// <returns>页面处理结果。</returns>
    public virtual async Task<IActionResult> OnGetAsync()
    {
        ProfileManagementPageCreationContext = new ProfileManagementPageCreationContext(LazyServiceProvider);

        foreach (var contributor in Options.Contributors)
        {
            await contributor.ConfigureAsync(ProfileManagementPageCreationContext);
        }

        if (ReturnUrl != null)
        {
            if (!Url.IsLocalUrl(ReturnUrl) &&
                !ReturnUrl.StartsWith(UriHelper.BuildAbsolute(Request.Scheme, Request.Host, Request.PathBase).RemovePostFix("/")) &&
                !await AppUrlProvider.IsRedirectAllowedUrlAsync(ReturnUrl))
            {
                ReturnUrl = null;
            }
        }

        return Page();
    }

    /// <summary>
    /// 异步处理页面 POST 请求。
    /// </summary>
    /// <returns>页面处理结果。</returns>
    public virtual Task<IActionResult> OnPostAsync()
    {
        return Task.FromResult<IActionResult>(Page());
    }
}
