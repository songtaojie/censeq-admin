using System.ComponentModel.DataAnnotations;

namespace Censeq.SettingManagement;

public class CreateSettingDefinitionDto
{
    [Required]
    [MaxLength(128)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(128)]
    public string DisplayName { get; set; } = string.Empty;

    [MaxLength(512)]
    public string? Description { get; set; }

    [MaxLength(512)]
    public string? DefaultValue { get; set; }

    [MaxLength(512)]
    public string? CurrentValue { get; set; }

    public bool IsVisibleToClients { get; set; }
}
