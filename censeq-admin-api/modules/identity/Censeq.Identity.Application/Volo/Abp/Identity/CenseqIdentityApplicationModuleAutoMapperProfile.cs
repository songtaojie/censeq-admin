using AutoMapper;
using Censeq.Identity.Application.Contracts.Censeq.Identity;

namespace Censeq.Identity;

public class CenseqIdentityApplicationModuleAutoMapperProfile : Profile
{
    public CenseqIdentityApplicationModuleAutoMapperProfile()
    {
        CreateMap<IdentityUser, IdentityUserDto>()
            .MapExtraProperties();

        CreateMap<IdentityRole, IdentityRoleDto>()
            .MapExtraProperties();
    }
}
