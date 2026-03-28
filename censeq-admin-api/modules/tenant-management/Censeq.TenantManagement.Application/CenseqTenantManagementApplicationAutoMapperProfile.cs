using AutoMapper;
using Censeq.TenantManagement.Entities;

namespace Censeq.TenantManagement;

public class CenseqTenantManagementApplicationAutoMapperProfile : Profile
{
    public CenseqTenantManagementApplicationAutoMapperProfile()
    {
        CreateMap<Tenant, TenantDto>()
            .MapExtraProperties();
    }
}
