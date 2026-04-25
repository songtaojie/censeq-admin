using System.ComponentModel.DataAnnotations;

namespace Censeq.Identity;

/// <summary>
/// 组织单元更新数据传输对象
/// </summary>
public class OrganizationUnitUpdateDto
{
    [Required]
    [StringLength(128)]
    public string DisplayName { get; set; } = default!;

    public int Status { get; set; } = 1;

    [StringLength(512)]
    public string? Remark { get; set; }
}
