using AutoMapper;
using Starshine.Admin.Web.Pages.Account.Components.ProfileManagementGroup.PersonalInfo;
using Starshine.Admin.Application.Contracts.Dtos.Profiles;

namespace Starshine.Admin.Web;

public class AbpAccountWebAutoMapperProfile : Profile
{
    public AbpAccountWebAutoMapperProfile()
    {
        CreateMap<ProfileOutput, AccountProfilePersonalInfoManagementGroupViewComponent.PersonalInfoModel>()
            .MapExtraProperties();
    }
}
