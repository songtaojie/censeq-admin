using AutoMapper;
using Censeq.Identity.Entities;

namespace Censeq.Identity;

public class CenseqIdentityApplicationModuleAutoMapperProfile : Profile
{
    public CenseqIdentityApplicationModuleAutoMapperProfile()
    {
        CreateMap<IdentityUser, IdentityUserDto>()
            .MapExtraProperties();

        CreateMap<IdentityRole, IdentityRoleDto>()
            .MapExtraProperties();

        CreateMap<OrganizationUnit, OrganizationUnitDto>()
            .MapExtraProperties();
    }
}
