using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace Censeq.Admin.Controllers;

[RemoteService(Name = "Admin")]
[Area("admin")]
[Route("api/admin/runtime-menus")]
public class CurrentUserMenuController : AdminController, Menus.ICurrentUserMenuAppService
{
    private readonly Menus.ICurrentUserMenuAppService _currentUserMenuAppService;

    public CurrentUserMenuController(Menus.ICurrentUserMenuAppService currentUserMenuAppService)
    {
        _currentUserMenuAppService = currentUserMenuAppService;
    }

    [HttpGet("current-user")]
    public virtual Task<Menus.CurrentUserMenuResultDto> GetAsync()
    {
        return _currentUserMenuAppService.GetAsync();
    }
}