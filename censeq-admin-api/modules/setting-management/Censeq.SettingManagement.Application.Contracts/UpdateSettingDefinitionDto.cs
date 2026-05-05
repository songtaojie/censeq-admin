using System.ComponentModel.DataAnnotations;

namespace Censeq.SettingManagement;

public class UpdateSettingDefinitionDto
{
    [Required]
    [MaxLength(128)]
    public string DisplayName { get; set; } = string.Empty;

    [MaxLength(512)]
    public string? Description { get; set; }

    [MaxLength(512)]
    public string? DefaultValue { get; set; }

    /// <summary>当前全局值，传 null 则清除</summary>
    [MaxLength(512)]
    public string? CurrentValue { get; set; }

    public bool IsVisibleToClients { get; set; }
}
