using Volo.Abp.Application.Dtos;

namespace Censeq.Identity;

public class GetIdentityUsersInput : ExtensiblePagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}
