using AutoMapper;
using Censeq.Identity.Entities;

namespace Censeq.Account;

public class CenseqAccountApplicationModuleAutoMapperProfile : Profile
{
    public CenseqAccountApplicationModuleAutoMapperProfile()
    {
        CreateMap<IdentityUser, ProfileDto>()
            .ForMember(dest => dest.HasPassword,
                op => op.MapFrom(src => src.PasswordHash != null))
            .MapExtraProperties();
    }
}
