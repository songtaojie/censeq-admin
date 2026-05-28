using AutoMapper;
using Censeq.Identity.Entities;
using Volo.Abp.Data;

namespace Censeq.Identity;

/// <summary>
/// Censeq身份应用模块AutoMapper配置
/// </summary>
public class CenseqIdentityApplicationModuleAutoMapperProfile : Profile
{
    public CenseqIdentityApplicationModuleAutoMapperProfile()
    {
        CreateMap<IdentityUser, IdentityUserDto>()
            .ForMember(dest => dest.AvatarUrl,
                opt => opt.MapFrom(src => src.GetProperty<string>("AvatarUrl", null)))
            .MapExtraProperties();

        CreateMap<IdentityRole, IdentityRoleDto>()
            .MapExtraProperties();

        CreateMap<OrganizationUnit, OrganizationUnitDto>()
            .MapExtraProperties();

        CreateMap<IdentitySession, IdentitySessionDto>()
            .ForMember(dest => dest.IsCurrentSession, opt => opt.Ignore());

        CreateMap<IdentitySecurityLog, IdentitySecurityLogDto>();
    }
}
