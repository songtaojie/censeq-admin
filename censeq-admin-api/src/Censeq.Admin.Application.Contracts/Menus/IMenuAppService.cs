using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Censeq.Admin.Menus;

public interface IMenuAppService : IApplicationService
{
    Task<ListResultDto<MenuPermissionGroupDto>> GetPermissionGroupsAsync(Guid? menuId = null, Guid? parentId = null);

    Task<ListResultDto<string>> GetReferencedPermissionNamesAsync();

    Task<ListResultDto<MenuTreeItemDto>> GetTreeAsync();

    Task<MenuDetailDto> GetAsync(Guid id);

    Task<MenuDetailDto> CreateAsync(CreateMenuDto input);

    Task<MenuDetailDto> UpdateAsync(Guid id, UpdateMenuDto input);

    Task DeleteAsync(Guid id);

    Task<MenuDetailDto> SetStatusAsync(Guid id, SetMenuStatusDto input);

    Task<MenuDetailDto> MoveAsync(Guid id, MenuSortDto input);

    Task CopyFromHostAsync(CopyTenantMenusDto input);
}