using Volo.Abp.Application.Dtos;

namespace Censeq.Identity.Application.Contracts.Censeq.Identity;

public class GetIdentityUsersInput : ExtensiblePagedAndSortedResultRequestDto
{
    public string Filter { get; set; }
}
