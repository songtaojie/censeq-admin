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

        CreateMap<IdentitySession, IdentitySessionDto>()
            .ForMember(dest => dest.IsCurrentSession, opt => opt.Ignore());
    }
}
