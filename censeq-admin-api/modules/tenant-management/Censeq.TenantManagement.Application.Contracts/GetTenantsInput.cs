using Volo.Abp.Application.Dtos;

namespace Censeq.TenantManagement;

public class GetTenantsInput : PagedAndSortedResultRequestDto
{
    public string Filter { get; set; }
}
