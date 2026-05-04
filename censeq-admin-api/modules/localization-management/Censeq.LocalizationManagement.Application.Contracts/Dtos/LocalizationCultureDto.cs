using System;
using Volo.Abp.Application.Dtos;

namespace Censeq.LocalizationManagement.Dtos;

public class LocalizationCultureDto : AuditedEntityDto<Guid>
{
    public Guid? TenantId { get; set; }
    public string CultureName { get; set; } = null!;
    public string? UiCultureName { get; set; }
    public string DisplayName { get; set; } = null!;
    public bool IsEnabled { get; set; }
}
