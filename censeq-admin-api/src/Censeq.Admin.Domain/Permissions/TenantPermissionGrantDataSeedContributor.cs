using System;
using System.Linq;
using System.Threading.Tasks;
using Censeq.Admin.Permissions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;

namespace Censeq.Admin.Permissions;

/// <summary>
/// Seeds the platform-defined permission scope for a newly created tenant.
/// </summary>
public class TenantPermissionGrantDataSeedContributor : DomainService, IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<TenantPermissionGrant, Guid> _tenantPermissionGrantRepository;

    public TenantPermissionGrantDataSeedContributor(
        IRepository<TenantPermissionGrant, Guid> tenantPermissionGrantRepository)
    {
        _tenantPermissionGrantRepository = tenantPermissionGrantRepository;
    }

    [UnitOfWork]
    public virtual async Task SeedAsync(DataSeedContext context)
    {
        if (!context.TenantId.HasValue)
        {
            return;
        }

        var tenantId = context.TenantId.Value;
        var existingGrants = await _tenantPermissionGrantRepository.GetListAsync(x => x.TenantId == tenantId);
        if (existingGrants.Count > 0)
        {
            return;
        }

        var grants = AdminSeedPermissionNames.TenantAdminDefaults
            .Select(permissionName => new TenantPermissionGrant(GuidGenerator.Create(), tenantId, permissionName))
            .ToList();

        if (grants.Count == 0)
        {
            return;
        }

        await _tenantPermissionGrantRepository.InsertManyAsync(grants, autoSave: true);
    }
}
