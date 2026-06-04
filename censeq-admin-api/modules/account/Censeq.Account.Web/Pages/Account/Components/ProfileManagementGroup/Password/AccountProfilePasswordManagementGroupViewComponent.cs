using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Auditing;
using Censeq.Identity;
using Volo.Abp.Validation;

namespace Censeq.Account.Web.Pages.Account.Components.ProfileManagementGroup.Password;

/// <summary>
/// 账户个人资料密码管理分组视图组件。
/// </summary>
public class AccountProfilePasswordManagementGroupViewComponent : AbpViewComponent
{
    /// <summary>
    /// 个人资料应用服务。
    /// </summary>
    protected IProfileAppService ProfileAppService { get; }

    /// <summary>
    /// 初始化 AccountProfilePasswordManagementGroupViewComponent 实例。
    /// </summary>
    /// <param name="profileAppService">个人资料应用服务。</param>
    public AccountProfilePasswordManagementGroupViewComponent(
        IProfileAppService profileAppService)
    {
        ProfileAppService = profileAppService;
    }

    /// <summary>
    /// 异步调用视图组件。
    /// </summary>
    /// <returns>视图组件结果。</returns>
    public virtual async Task<IViewComponentResult> InvokeAsync()
    {
        var user = await ProfileAppService.GetAsync();

        var model = new ChangePasswordInfoModel
        {
            HideOldPasswordInput = !user.HasPassword
        };

        return View("~/Pages/Account/Components/ProfileManagementGroup/Password/Default.cshtml", model);
    }

    /// <summary>
    /// 修改 密码 信息 模型。
    /// </summary>
    public class ChangePasswordInfoModel
    {
        /// <summary>
        /// 当前密码。
        /// </summary>
        [Required]
        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPasswordLength))]
        [Display(Name = "DisplayName:CurrentPassword")]
        [DataType(DataType.Password)]
        [DisableAuditing]
        public string? CurrentPassword { get; set; }

        /// <summary>
        /// 新密码。
        /// </summary>
        [Required]
        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPasswordLength))]
        [Display(Name = "DisplayName:NewPassword")]
        [DataType(DataType.Password)]
        [DisableAuditing]
        public string? NewPassword { get; set; }

        /// <summary>
        /// 新密码确认。
        /// </summary>
        [Required]
        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPasswordLength))]
        [Display(Name = "DisplayName:NewPasswordConfirm")]
        [DataType(DataType.Password)]
        [DisableAuditing]
        public string? NewPasswordConfirm { get; set; }

        /// <summary>
        /// 是否隐藏旧密码输入。
        /// </summary>
        public bool HideOldPasswordInput { get; set; }
    }
}
