using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Censeq.Account;
using Censeq.Account.Localization;
using Volo.Abp.AspNetCore.ExceptionHandling;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.ExceptionHandling;
using Censeq.Identity;
using IdentityUser = Censeq.Identity.Entities.IdentityUser;

namespace Censeq.Account.Web.Pages.Account;

/// <summary>
/// 账户页面模型基类。
/// </summary>
public abstract class AccountPageModel : AbpPageModel
{
    /// <summary>
    /// 账户应用服务。
    /// </summary>
    protected IAccountAppService AccountAppService  =>
         LazyServiceProvider.LazyGetRequiredService<IAccountAppService>();
    /// <summary>
    /// 登录管理器。
    /// </summary>
    protected SignInManager<IdentityUser> SignInManager =>
            LazyServiceProvider.LazyGetRequiredService<SignInManager<IdentityUser>>();
    /// <summary>
    /// 用户管理器。
    /// </summary>
    protected IdentityUserManager UserManager =>
            LazyServiceProvider.LazyGetRequiredService<IdentityUserManager>();
    /// <summary>
    /// Identity 安全日志管理器。
    /// </summary>
    protected IdentitySecurityLogManager IdentitySecurityLogManager =>
            LazyServiceProvider.LazyGetRequiredService<IdentitySecurityLogManager>();
    /// <summary>
    /// Identity 配置项。
    /// </summary>
    protected IOptions<IdentityOptions> IdentityOptions =>
            LazyServiceProvider.LazyGetRequiredService<IOptions<IdentityOptions>>();
    /// <summary>
    /// 异常错误信息转换器。
    /// </summary>
    protected IExceptionToErrorInfoConverter ExceptionToErrorInfoConverter =>
            LazyServiceProvider.LazyGetRequiredService<IExceptionToErrorInfoConverter>();

    /// <summary>
    /// 初始化 AccountPageModel 实例。
    /// </summary>
    protected AccountPageModel()
    {
        LocalizationResourceType = typeof(AccountResource);
        ObjectMapperContext = typeof(CenseqAccountWebModule);
    }

    /// <summary>
    /// Check Current 租户。
    /// </summary>
    /// <param name="tenantId">租户标识。</param>
    protected virtual void CheckCurrentTenant(Guid? tenantId)
    {
        if (CurrentTenant.Id != tenantId)
        {
            throw new ApplicationException($"Current tenant is different than given tenant. CurrentTenant.Id: {CurrentTenant.Id}, given tenantId: {tenantId}");
        }
    }

    /// <summary>
    /// Check Identity Errors。
    /// </summary>
    /// <param name="identityResult">Identity 结果。</param>
    protected virtual void CheckIdentityErrors(IdentityResult identityResult)
    {
        if (!identityResult.Succeeded)
        {
            throw new UserFriendlyException("Operation failed: " + identityResult.Errors.Select(e => $"[{e.Code}] {e.Description}").JoinAsString(", "));
        }

        //identityResult.CheckErrors(LocalizationManager); //TODO: Get from old Abp
    }

    /// <summary>
    /// Get Localize Exception Message。
    /// </summary>
    /// <param name="exception">exception。</param>
    /// <returns>返回结果。</returns>
    protected virtual string GetLocalizeExceptionMessage(Exception exception)
    {
        if (exception is ILocalizeErrorMessage || exception is IHasErrorCode)
        {
            return ExceptionToErrorInfoConverter.Convert(exception).Message ?? string.Empty;
        }

        return exception.Message;
    }
}
