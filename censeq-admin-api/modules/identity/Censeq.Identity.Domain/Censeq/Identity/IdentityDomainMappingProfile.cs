using AutoMapper;
using Censeq.Identity.Entities;
using Volo.Abp.Users;

namespace Censeq.Identity;

/// <summary>
/// 身份领域映射配置
/// </summary>
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
