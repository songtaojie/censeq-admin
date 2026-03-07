using AutoMapper;
using Censeq.Admin.Web.Pages.Account.Components.ProfileManagementGroup.PersonalInfo;
using Censeq.Admin.Application.Contracts.Dtos.Profiles;

namespace Censeq.Admin.Web;

public class AbpAccountWebAutoMapperProfile : Profile
{
    public AbpAccountWebAutoMapperProfile()
    {
        CreateMap<ProfileOutput, AccountProfilePersonalInfoManagementGroupViewComponent.PersonalInfoModel>()
            .MapExtraProperties();
    }
}
