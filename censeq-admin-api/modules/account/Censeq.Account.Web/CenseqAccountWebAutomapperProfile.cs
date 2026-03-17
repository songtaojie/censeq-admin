using AutoMapper;
using Censeq.Account.Web.Pages.Account.Components.ProfileManagementGroup.PersonalInfo;
using Censeq.Account;

namespace Censeq.Account.Web;

public class CenseqAccountWebAutomapperProfile : Profile
{
    public CenseqAccountWebAutomapperProfile()
    {
        CreateMap<ProfileDto, AccountProfilePersonalInfoManagementGroupViewComponent.PersonalInfoModel>()
            .MapExtraProperties();
    }
}
