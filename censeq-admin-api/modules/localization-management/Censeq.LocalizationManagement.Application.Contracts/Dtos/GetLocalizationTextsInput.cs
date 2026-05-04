using Volo.Abp.Application.Dtos;

namespace Censeq.LocalizationManagement.Dtos;

public class GetLocalizationTextsInput : PagedAndSortedResultRequestDto
{
    public string? ResourceName { get; set; }
    public string? CultureName { get; set; }
    public string? Filter { get; set; }
}
