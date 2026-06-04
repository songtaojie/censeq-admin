using AutoMapper;
using Censeq.Account.Web.Pages.Account.Components.ProfileManagementGroup.PersonalInfo;
using Censeq.Account;

namespace Censeq.Account.Web;

/// <summary>
/// 账户 Web AutoMapper 配置。
/// </summary>
public class CenseqAccountWebAutomapperProfile : Profile
{
    /// <summary>
    /// 初始化 CenseqAccountWebAutomapperProfile 实例。
    /// </summary>
    public CenseqAccountWebAutomapperProfile()
    {
        CreateMap<ProfileDto, AccountProfilePersonalInfoManagementGroupViewComponent.PersonalInfoModel>()
            .MapExtraProperties();
    }
}
