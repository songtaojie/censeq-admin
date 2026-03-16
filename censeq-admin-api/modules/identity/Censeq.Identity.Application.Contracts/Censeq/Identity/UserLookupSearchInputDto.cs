using Volo.Abp.Application.Dtos;

namespace Censeq.Identity;

public class UserLookupSearchInputDto : ExtensiblePagedAndSortedResultRequestDto
{
    public string Filter { get; set; }
}
