using System.ComponentModel.DataAnnotations;

namespace Censeq.LocalizationManagement.Dtos;

public class CreateUpdateLocalizationCultureDto
{
    [Required]
    [MaxLength(10)]
    public string CultureName { get; set; } = null!;

    [MaxLength(10)]
    public string? UiCultureName { get; set; }

    [Required]
    [MaxLength(128)]
    public string DisplayName { get; set; } = null!;

    public bool IsEnabled { get; set; } = true;
}
