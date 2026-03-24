using AutoMapper;
using Censeq.Identity.Entities;
using Volo.Abp.Users;

namespace Censeq.Identity;

public class IdentityDomainMappingProfile : Profile
{
    public IdentityDomainMappingProfile()
    {
        CreateMap<IdentityUser, UserEto>();
        CreateMap<IdentityClaimType, IdentityClaimTypeEto>();
        CreateMap<IdentityRole, IdentityRoleEto>();
        CreateMap<OrganizationUnit, OrganizationUnitEto>();
    }
}
