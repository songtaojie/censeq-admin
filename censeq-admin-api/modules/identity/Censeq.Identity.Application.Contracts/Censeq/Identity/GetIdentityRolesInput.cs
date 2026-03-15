using Volo.Abp.Application.Dtos;

namespace Censeq.Identity.Application.Contracts.Censeq.Identity;

public class GetIdentityRolesInput : ExtensiblePagedAndSortedResultRequestDto
{
    public string Filter { get; set; }
}
