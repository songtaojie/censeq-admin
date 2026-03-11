using AutoMapper;
using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;

namespace Censeq.TenantManagement;

public class CenseqTenantManagementDomainMappingProfile : Profile
{
    public CenseqTenantManagementDomainMappingProfile()
    {
        CreateMap<Tenant, TenantConfiguration>()
            .ForMember(ti => ti.ConnectionStrings, opts =>
            {
                opts.MapFrom((tenant, ti) =>
                {
                    var connStrings = new ConnectionStrings();

                    if (tenant.ConnectionStrings == null)
                    {
                        return connStrings;
                    }

                    foreach (var connectionString in tenant.ConnectionStrings)
                    {
                        connStrings[connectionString.Name] = connectionString.Value;
                    }

                    return connStrings;
                });
            })
            .ForMember(x => x.IsActive, x => x.Ignore());

        CreateMap<Tenant, TenantEto>();
    }
}
