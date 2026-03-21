using Volo.Abp.Domain.Entities;
using Volo.Abp.ObjectExtending;

namespace Censeq.Account;

public class ProfileDto : ExtensibleObject, IHasConcurrencyStamp
{
    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Surname { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public bool IsExternal { get; set; }

    public bool HasPassword { get; set; }

    public string ConcurrencyStamp { get; set; } = string.Empty;
}
