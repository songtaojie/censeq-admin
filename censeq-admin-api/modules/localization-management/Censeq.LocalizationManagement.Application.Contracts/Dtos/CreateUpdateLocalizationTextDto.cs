using System.ComponentModel.DataAnnotations;

namespace Censeq.LocalizationManagement.Dtos;

public class CreateLocalizationTextDto
{
    [Required]
    [MaxLength(128)]
    public string ResourceName { get; set; } = null!;

    [Required]
    [MaxLength(10)]
    public string CultureName { get; set; } = null!;

    [Required]
    [MaxLength(512)]
    public string Key { get; set; } = null!;

    public string? Value { get; set; }
}

public class UpdateLocalizationTextDto
{
    public string? Value { get; set; }
}
