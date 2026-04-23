using System.ComponentModel.DataAnnotations;

namespace Censeq.Identity;

public class OrganizationUnitUpdateDto
{
    [Required]
    [StringLength(128)]
    public string DisplayName { get; set; } = default!;

    public int Status { get; set; } = 1;

    [StringLength(512)]
    public string? Remark { get; set; }
}
