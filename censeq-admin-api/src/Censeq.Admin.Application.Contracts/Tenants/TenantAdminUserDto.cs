namespace Censeq.Admin.Tenants;

/// <summary>
/// 租户管理员账号摘要信息
/// </summary>
public class TenantAdminUserDto
{
    public Guid TenantId { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
}
