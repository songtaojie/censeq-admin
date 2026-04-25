using System.ComponentModel.DataAnnotations;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Validation;

namespace Censeq.Identity;

/// <summary>
/// 身份角色创建Or更新数据传输对象基类
/// </summary>
public class IdentityRoleCreateOrUpdateDtoBase : ExtensibleObject
{
    [Required]
    [DynamicStringLength(typeof(IdentityRoleConsts), nameof(IdentityRoleConsts.MaxNameLength))]
    [Display(Name = "RoleName")]
    public string Name { get; set; }

    [DynamicStringLength(typeof(IdentityRoleConsts), nameof(IdentityRoleConsts.MaxCodeLength))]
    [Display(Name = "RoleCode")]
    public string? Code { get; set; }

    public bool IsDefault { get; set; }

    public bool IsPublic { get; set; }

    protected IdentityRoleCreateOrUpdateDtoBase() : base(false)
    {

    }
}
