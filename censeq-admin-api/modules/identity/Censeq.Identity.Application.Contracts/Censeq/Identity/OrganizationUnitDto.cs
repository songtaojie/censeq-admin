using System;
using Volo.Abp.Application.Dtos;

namespace Censeq.Identity;

public class OrganizationUnitDto : ExtensibleEntityDto<Guid>
{
    public Guid? ParentId { get; set; }

    public string Code { get; set; } = default!;

    public string DisplayName { get; set; } = default!;

    public int EntityVersion { get; set; }

    public int Status { get; set; } = 1;

    public string? Remark { get; set; }
}
