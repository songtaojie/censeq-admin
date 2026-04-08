using Volo.Abp.Application.Dtos;

namespace Censeq.Identity;

public class GetIdentityClaimTypesInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}