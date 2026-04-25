using System;
using System.Threading.Tasks;
using IdentityUser = Censeq.Identity.Entities.IdentityUser;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace Censeq.Identity.AspNetCore;

/// <summary>
/// Censeq 安全戳验证器
/// </summary>
public class CenseqSecurityStampValidator : SecurityStampValidator<IdentityUser>
{
    /// <summary>
    /// 租户配置提供程序
    /// </summary>
    protected ITenantConfigurationProvider TenantConfigurationProvider { get; }

    /// <summary>
    /// 当前租户
    /// </summary>
    protected ICurrentTenant CurrentTenant { get; }

    /// <summary>
    /// 初始化 <see cref="CenseqSecurityStampValidator"/> 类的新实例
    /// </summary>
    /// <param name="options">安全戳验证器选项</param>
    /// <param name="signInManager">登录管理器</param>
    /// <param name="loggerFactory">日志工厂</param>
    /// <param name="tenantConfigurationProvider">租户配置提供程序</param>
    /// <param name="currentTenant">当前租户</param>
    public CenseqSecurityStampValidator(
        IOptions<SecurityStampValidatorOptions> options,
        SignInManager<IdentityUser> signInManager,
        ILoggerFactory loggerFactory,
        ITenantConfigurationProvider tenantConfigurationProvider,
        ICurrentTenant currentTenant)
        : base(
            options,
            signInManager,
            loggerFactory)
    {
        TenantConfigurationProvider = tenantConfigurationProvider;
        CurrentTenant = currentTenant;
    }

    /// <summary>
    /// 验证安全戳
    /// </summary>
    /// <param name="context">Cookie 验证主体上下文</param>
    /// <returns>异步任务</returns>
    [UnitOfWork]
    /// <summary>
    /// override Task
    /// </summary>
    public async override Task ValidateAsync(CookieValidatePrincipalContext context)
    {
        TenantConfiguration? tenant = null;
        try
        {
            tenant = await TenantConfigurationProvider.GetAsync(saveResolveResult: false);
        }
        catch (Exception e)
        {
            Logger.LogException(e);
        }

        using (CurrentTenant.Change(tenant?.Id, tenant?.Name))
        {
            await base.ValidateAsync(context);
        }
    }
}
