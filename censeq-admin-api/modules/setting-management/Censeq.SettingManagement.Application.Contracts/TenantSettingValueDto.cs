using System;

namespace Censeq.SettingManagement;

public class TenantSettingValueDto
{
    public Guid TenantId { get; set; }

    public string? Value { get; set; }
}
