using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Censeq.Admin.Controllers;

[RemoteService(Name = "Admin")]
[Area("admin")]
[Route("api/admin/menus")]
public class MenuController : AdminController, Menus.IMenuAppService
{
    private readonly Menus.IMenuAppService _menuAppService;

    public MenuController(Menus.IMenuAppService menuAppService)
    {
        _menuAppService = menuAppService;
    }

    [HttpGet("permissions")]
    public virtual Task<ListResultDto<Menus.MenuPermissionGroupDto>> GetPermissionGroupsAsync([FromQuery] Guid? menuId = null, [FromQuery] Guid? parentId = null)
    {
        return _menuAppService.GetPermissionGroupsAsync(menuId, parentId);
    }

    [HttpGet("referenced-permissions")]
    public virtual Task<ListResultDto<string>> GetReferencedPermissionNamesAsync()
    {
        return _menuAppService.GetReferencedPermissionNamesAsync();
    }

    [HttpGet("tenant-scope-permissions")]
    public virtual Task<ListResultDto<string>> GetTenantScopePermissionNamesAsync()
    {
        return _menuAppService.GetTenantScopePermissionNamesAsync();
    }

    [HttpGet("tree")]
    public virtual Task<ListResultDto<Menus.MenuTreeItemDto>> GetTreeAsync()
    {
        return _menuAppService.GetTreeAsync();
    }

    [HttpGet("{id:guid}")]
    public virtual Task<Menus.MenuDetailDto> GetAsync(Guid id)
    {
        return _menuAppService.GetAsync(id);
    }

    [HttpPost]
    public virtual Task<Menus.MenuDetailDto> CreateAsync([FromBody] Menus.CreateMenuDto input)
    {
        return _menuAppService.CreateAsync(input);
    }

    [HttpPut("{id:guid}")]
    public virtual Task<Menus.MenuDetailDto> UpdateAsync(Guid id, [FromBody] Menus.UpdateMenuDto input)
    {
        return _menuAppService.UpdateAsync(id, input);
    }

    [HttpDelete("{id:guid}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return _menuAppService.DeleteAsync(id);
    }

    [HttpPut("{id:guid}/status")]
    public virtual Task<Menus.MenuDetailDto> SetStatusAsync(Guid id, [FromBody] Menus.SetMenuStatusDto input)
    {
        return _menuAppService.SetStatusAsync(id, input);
    }

    [HttpPut("{id:guid}/sort")]
    public virtual Task<Menus.MenuDetailDto> MoveAsync(Guid id, [FromBody] Menus.MenuSortDto input)
    {
        return _menuAppService.MoveAsync(id, input);
    }

    [HttpPost("copy-from-host")]
    public virtual Task CopyFromHostAsync([FromBody] Menus.CopyTenantMenusDto input)
    {
        return _menuAppService.CopyFromHostAsync(input);
    }
}