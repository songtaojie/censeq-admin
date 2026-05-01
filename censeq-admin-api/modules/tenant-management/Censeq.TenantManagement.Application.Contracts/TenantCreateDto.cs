using System.ComponentModel.DataAnnotations;
using Volo.Abp.Auditing;

namespace Censeq.TenantManagement;

public class TenantCreateDto : TenantCreateOrUpdateDtoBase
{
    [Required]
    [MaxLength(256)]
    public virtual string AdminUserName { get; set; } = "admin";

    [Required]
    [MaxLength(64)]
    public virtual string AdminName { get; set; } = "admin";

    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public virtual string AdminEmailAddress { get; set; }

    [Required]
    [MaxLength(128)]
    [DisableAuditing]
    public virtual string AdminPassword { get; set; }
}
