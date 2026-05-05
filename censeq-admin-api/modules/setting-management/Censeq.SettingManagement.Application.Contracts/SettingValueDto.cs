using System.Collections.Generic;

namespace Censeq.SettingManagement;

public class SettingValueDto
{
    public string Name { get; set; } = default!;

    public string? GlobalValue { get; set; }

    public List<TenantSettingValueDto> TenantValues { get; set; } = new();

    public List<UserSettingValueDto> UserValues { get; set; } = new();
}
