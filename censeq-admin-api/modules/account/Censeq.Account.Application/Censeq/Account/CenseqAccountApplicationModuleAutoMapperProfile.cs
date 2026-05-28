using AutoMapper;
using Censeq.Identity.Entities;
using Volo.Abp.Data;

namespace Censeq.Account;

public class CenseqAccountApplicationModuleAutoMapperProfile : Profile
{
    public CenseqAccountApplicationModuleAutoMapperProfile()
    {
        CreateMap<IdentityUser, ProfileDto>()
            .ForMember(dest => dest.HasPassword,
                op => op.MapFrom(src => src.PasswordHash != null))
            .ForMember(dest => dest.AvatarUrl,
                op => op.MapFrom(src => src.GetProperty<string>("AvatarUrl", null)))
            .MapExtraProperties();
    }
}
