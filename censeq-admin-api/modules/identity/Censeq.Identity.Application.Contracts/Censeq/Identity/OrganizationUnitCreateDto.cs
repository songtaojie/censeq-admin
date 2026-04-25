using System;
using System.ComponentModel.DataAnnotations;

namespace Censeq.Identity;

/// <summary>
/// 组织单元创建数据传输对象
/// </summary>
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
