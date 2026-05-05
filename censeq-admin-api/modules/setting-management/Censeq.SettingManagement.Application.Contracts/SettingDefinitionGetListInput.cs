using Volo.Abp.Application.Dtos;

namespace Censeq.SettingManagement;

public class SettingDefinitionGetListInput : PagedResultRequestDto
{
    public string? Filter { get; set; }
}
