using AutoMapper;
using Censeq.Identity.Entities;
using Volo.Abp.Data;

namespace Censeq.Account;

/// <summary>
/// 账户应用程序 AutoMapper 配置。
/// </summary>
public class CenseqAccountApplicationModuleAutoMapperProfile : Profile
{
    /// <summary>
    /// 初始化 CenseqAccountApplicationModuleAutoMapperProfile 实例。
    /// </summary>
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
