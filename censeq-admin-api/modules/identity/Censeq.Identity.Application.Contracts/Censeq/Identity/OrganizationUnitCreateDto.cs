using System;
using System.ComponentModel.DataAnnotations;

namespace Censeq.Identity;

public class OrganizationUnitCreateDto
{
    [Required]
    [StringLength(128)]
    public string DisplayName { get; set; } = default!;

    public Guid? ParentId { get; set; }
}
