using Volo.Abp.Domain.Entities;

namespace Censeq.Identity.Application.Contracts.Censeq.Identity;

public class IdentityRoleUpdateDto : IdentityRoleCreateOrUpdateDtoBase, IHasConcurrencyStamp
{
    public string ConcurrencyStamp { get; set; }
}
