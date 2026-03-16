using System.ComponentModel.DataAnnotations;

namespace Censeq.Identity;

public class IdentityUserUpdateRolesDto
{
    [Required]
    public string[] RoleNames { get; set; }
}
