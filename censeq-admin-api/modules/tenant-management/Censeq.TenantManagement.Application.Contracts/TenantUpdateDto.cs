using Volo.Abp.Domain.Entities;

namespace Censeq.TenantManagement;

public class TenantUpdateDto : TenantCreateOrUpdateDtoBase, IHasConcurrencyStamp
{
    public string ConcurrencyStamp { get; set; }
}
