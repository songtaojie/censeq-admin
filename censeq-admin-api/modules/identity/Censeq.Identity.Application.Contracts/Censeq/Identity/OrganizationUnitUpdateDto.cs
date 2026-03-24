using System.ComponentModel.DataAnnotations;

namespace Censeq.Identity;

public class OrganizationUnitUpdateDto
{
    [Required]
    [StringLength(128)]
    public string DisplayName { get; set; } = default!;
}
