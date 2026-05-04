using System.ComponentModel.DataAnnotations;

namespace Censeq.LocalizationManagement.Dtos;

public class CreateUpdateLocalizationResourceDto
{
    [Required]
    [MaxLength(128)]
    public string Name { get; set; } = null!;

    [MaxLength(128)]
    public string? DisplayName { get; set; }

    [MaxLength(10)]
    public string? DefaultCultureName { get; set; }
}
