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

    /// <summary>重置租户管理员密码</summary>
    [HttpPost("{id}/reset-admin-password")]
    public virtual Task ResetAdminPasswordAsync(Guid id, [FromBody] ResetTenantAdminPasswordDto input)
    {
        return _appService.ResetAdminPasswordAsync(id, input.NewPassword);
    }

    /// <summary>获取平台向指定租户开放的权限名称列表</summary>
    [HttpGet("{id}/permissions")]
    public virtual Task<List<string>> GetPermissionsAsync(Guid id)
    {
        return _appService.GetPermissionsAsync(id);
    }

    /// <summary>全量更新平台向指定租户开放的权限范围</summary>
    [HttpPut("{id}/permissions")]
    public virtual Task UpdatePermissionsAsync(Guid id, [FromBody] UpdateTenantPermissionsDto input)
    {
        return _appService.UpdatePermissionsAsync(id, input);
    }

    /// <summary>批量获取多个租户的管理员账号信息</summary>
    [HttpGet("admin-users")]
    public virtual Task<List<TenantAdminUserDto>> GetAdminUsersAsync([FromQuery] List<Guid> tenantIds)
    {
        return _appService.GetAdminUsersAsync(tenantIds);
    }
}
