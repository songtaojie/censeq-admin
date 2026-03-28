using System.Threading.Tasks;
using Censeq.TenantManagement.Entities;
using JetBrains.Annotations;
using Volo.Abp.Domain.Services;

namespace Censeq.TenantManagement;

public interface ITenantManager : IDomainService
{
    [NotNull]
    Task<Tenant> CreateAsync([NotNull] string name);

    Task ChangeNameAsync([NotNull] Tenant tenant, [NotNull] string name);
}
