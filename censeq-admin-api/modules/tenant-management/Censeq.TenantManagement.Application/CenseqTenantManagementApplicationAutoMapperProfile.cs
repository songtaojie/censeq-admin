using AutoMapper;

namespace Censeq.TenantManagement;

public class CenseqTenantManagementApplicationAutoMapperProfile : Profile
{
    public CenseqTenantManagementApplicationAutoMapperProfile()
    {
        CreateMap<Tenant, TenantDto>()
            .MapExtraProperties();
    }
}
