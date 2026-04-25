using Censeq.Admin.Tenants;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace Censeq.Admin.Controllers;

[RemoteService(Name = "Admin")]
[Area("admin")]
[Route("api/admin/tenants")]
public class AdminTenantController : AdminController
{
    private readonly AdminTenantAppService _appService;

    public AdminTenantController(AdminTenantAppService appService)
    {
        _appService = appService;
    }

    /// <summary>
    /// 重置租户管理员密码
    /// </summary>
    [HttpPost("{id}/reset-admin-password")]
    public virtual Task ResetAdminPasswordAsync(Guid id, [FromBody] ResetTenantAdminPasswordDto input)
    {
        return _appService.ResetAdminPasswordAsync(id, input.NewPassword);
    }
}
