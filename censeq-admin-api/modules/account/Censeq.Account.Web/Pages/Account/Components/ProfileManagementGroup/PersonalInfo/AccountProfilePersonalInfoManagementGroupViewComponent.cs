using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Widgets;
using Volo.Abp.Domain.Entities;
using Censeq.Identity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Validation;

namespace Censeq.Account.Web.Pages.Account.Components.ProfileManagementGroup.PersonalInfo;

/// <summary>
/// 账户个人资料信息管理分组视图组件。
/// </summary>
public class AccountProfilePersonalInfoManagementGroupViewComponent : AbpViewComponent
{
    /// <summary>
    /// 个人资料应用服务。
    /// </summary>
    protected IProfileAppService ProfileAppService { get; }

    /// <summary>
    /// 初始化 AccountProfilePersonalInfoManagementGroupViewComponent 实例。
    /// </summary>
    /// <param name="profileAppService">个人资料应用服务。</param>
    public AccountProfilePersonalInfoManagementGroupViewComponent(
        IProfileAppService profileAppService)
    {
        ProfileAppService = profileAppService;

        ObjectMapperContext = typeof(CenseqAccountWebModule);
    }

    /// <summary>
    /// 异步调用视图组件。
    /// </summary>
    /// <returns>视图组件结果。</returns>
    public virtual async Task<IViewComponentResult> InvokeAsync()
    {
        var user = await ProfileAppService.GetAsync();

        var model = ObjectMapper.Map<ProfileDto, PersonalInfoModel>(user);

        return View("~/Pages/Account/Components/ProfileManagementGroup/PersonalInfo/Default.cshtml", model);
    }

    /// <summary>
    /// 个人 信息 模型。
    /// </summary>
    public class PersonalInfoModel : ExtensibleObject, IHasConcurrencyStamp
    {
        /// <summary>
        /// 用户名。
        /// </summary>
        [Required]
        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxUserNameLength))]
        [Display(Name = "DisplayName:UserName")]
        public string? UserName { get; set; }

        /// <summary>
        /// 邮箱。
        /// </summary>
        [Required]
        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxEmailLength))]
        [Display(Name = "DisplayName:Email")]
        public string? Email { get; set; }

        /// <summary>
        /// 名。
        /// </summary>
        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxNameLength))]
        [Display(Name = "DisplayName:Name")]
        public string? Name { get; set; }

        /// <summary>
        /// 姓。
        /// </summary>
        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxSurnameLength))]
        [Display(Name = "DisplayName:Surname")]
        public string? Surname { get; set; }

        /// <summary>
        /// 手机号。
        /// </summary>
        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPhoneNumberLength))]
        [Display(Name = "DisplayName:PhoneNumber")]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// 并发标记。
        /// </summary>
        [HiddenInput] 
        public required string ConcurrencyStamp { get; set; }
    }
}