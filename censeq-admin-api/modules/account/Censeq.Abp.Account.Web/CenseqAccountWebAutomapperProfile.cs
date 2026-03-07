using AutoMapper;
using Censeq.Abp.Account.Web.Pages.Account.Components.ProfileManagementGroup.PersonalInfo;
using Volo.Abp.Account;

namespace Censeq.Abp.Account.Web;

public class StarshineAccountWebAutomapperProfile : Profile
{
    public StarshineAccountWebAutomapperProfile()
    {
        CreateMap<ProfileDto, AccountProfilePersonalInfoManagementGroupViewComponent.PersonalInfoModel>()
            .MapExtraProperties();
    }
}
