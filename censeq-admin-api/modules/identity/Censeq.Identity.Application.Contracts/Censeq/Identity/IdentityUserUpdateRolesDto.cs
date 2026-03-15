using System.ComponentModel.DataAnnotations;

namespace Censeq.Identity.Application.Contracts.Censeq.Identity;

public class IdentityUserUpdateRolesDto
{
    [Required]
    public string[] RoleNames { get; set; }
}
