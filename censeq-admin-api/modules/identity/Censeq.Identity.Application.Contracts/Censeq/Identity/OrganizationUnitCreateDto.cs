using System;
using System.ComponentModel.DataAnnotations;

namespace Censeq.Identity;

public class OrganizationUnitCreateDto
{
    [Required]
    [StringLength(128)]
    public string DisplayName { get; set; } = default!;

    public Guid? ParentId { get; set; }

    public int Status { get; set; } = 1;

    [StringLength(512)]
    public string? Remark { get; set; }
}
