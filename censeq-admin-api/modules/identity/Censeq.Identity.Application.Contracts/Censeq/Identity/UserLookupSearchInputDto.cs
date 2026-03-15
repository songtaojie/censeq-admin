using Volo.Abp.Application.Dtos;

namespace Censeq.Identity.Application.Contracts.Censeq.Identity;

public class UserLookupSearchInputDto : ExtensiblePagedAndSortedResultRequestDto
{
    public string Filter { get; set; }
}
