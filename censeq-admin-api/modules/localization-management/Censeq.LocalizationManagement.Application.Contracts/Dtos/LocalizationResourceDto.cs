using System;
using Volo.Abp.Application.Dtos;

namespace Censeq.LocalizationManagement.Dtos;

public class LocalizationResourceDto : AuditedEntityDto<Guid>
{
    public string Name { get; set; } = null!;
    public string? DisplayName { get; set; }
    public string? DefaultCultureName { get; set; }
}
