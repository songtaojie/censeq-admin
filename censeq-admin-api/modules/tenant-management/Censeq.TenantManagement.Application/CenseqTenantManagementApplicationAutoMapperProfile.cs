using AutoMapper;
using Censeq.TenantManagement.Entities;

namespace Censeq.TenantManagement;

public class CenseqTenantManagementApplicationAutoMapperProfile : Profile
{
    public CenseqTenantManagementApplicationAutoMapperProfile()
    {
        CreateMap<Tenant, TenantDto>()
            .ForMember(d => d.IsActive, opts => opts.MapFrom(s => s.IsActive))
            .ForMember(d => d.IsDeleted, opts => opts.MapFrom(s => s.IsDeleted))
            .MapExtraProperties();
    }
}
