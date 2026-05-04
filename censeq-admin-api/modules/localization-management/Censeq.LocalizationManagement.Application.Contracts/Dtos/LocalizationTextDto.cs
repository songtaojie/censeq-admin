using System;
using Volo.Abp.Application.Dtos;

namespace Censeq.LocalizationManagement.Dtos;

public class LocalizationTextDto : AuditedEntityDto<Guid>
{
    public Guid? TenantId { get; set; }
    public string ResourceName { get; set; } = null!;
    public string CultureName { get; set; } = null!;
    public string Key { get; set; } = null!;
    public string? Value { get; set; }
}
