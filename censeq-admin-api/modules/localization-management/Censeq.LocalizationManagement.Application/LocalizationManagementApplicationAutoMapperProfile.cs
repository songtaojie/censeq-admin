using AutoMapper;
using Censeq.LocalizationManagement.Dtos;
using Censeq.LocalizationManagement.Entities;

namespace Censeq.LocalizationManagement;

public class LocalizationManagementApplicationAutoMapperProfile : Profile
{
    public LocalizationManagementApplicationAutoMapperProfile()
    {
        CreateMap<LocalizationResource, LocalizationResourceDto>();
        CreateMap<LocalizationCulture, LocalizationCultureDto>();
        CreateMap<LocalizationText, LocalizationTextDto>();
    }
}
