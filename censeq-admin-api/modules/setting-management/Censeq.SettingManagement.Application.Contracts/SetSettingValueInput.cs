using System.ComponentModel.DataAnnotations;

namespace Censeq.SettingManagement;

public class SetSettingValueInput
{
    [Required]
    [MaxLength(128)]
    public string Name { get; set; } = default!;

    public string? Value { get; set; }

    /// <summary>ProviderName：全局 / 租户 / 用户</summary>
    [Required]
    [MaxLength(64)]
    public string ProviderName { get; set; } = default!;

    /// <summary>全局时为 null，租户/用户时为对应实体 ID（字符串）</summary>
    public string? ProviderKey { get; set; }
}
