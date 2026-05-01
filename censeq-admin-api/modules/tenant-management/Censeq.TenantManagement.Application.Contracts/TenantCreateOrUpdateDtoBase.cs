using System.ComponentModel.DataAnnotations;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Validation;

namespace Censeq.TenantManagement;

public abstract class TenantCreateOrUpdateDtoBase : ExtensibleObject
{
    [Required]
    [DynamicStringLength(typeof(TenantConsts), nameof(TenantConsts.MaxNameLength))]
    [Display(Name = "TenantName")]
    public string Name { get; set; }

    [DynamicStringLength(typeof(TenantConsts), nameof(TenantConsts.MaxCodeLength))]
    [Display(Name = "TenantCode")]
    public string? Code { get; set; }

    [DynamicStringLength(typeof(TenantConsts), nameof(TenantConsts.MaxDomainLength))]
    [Display(Name = "TenantDomain")]
    public string? Domain { get; set; }

    [DynamicStringLength(typeof(TenantConsts), nameof(TenantConsts.MaxIconLength))]
    [Display(Name = "TenantIcon")]
    public string? Icon { get; set; }

    [DynamicStringLength(typeof(TenantConsts), nameof(TenantConsts.MaxCopyrightLength))]
    [Display(Name = "TenantCopyright")]
    public string? Copyright { get; set; }

    [DynamicStringLength(typeof(TenantConsts), nameof(TenantConsts.MaxIcpNoLength))]
    [Display(Name = "TenantIcpNo")]
    public string? IcpNo { get; set; }

    [DynamicStringLength(typeof(TenantConsts), nameof(TenantConsts.MaxIcpAddressLength))]
    [Display(Name = "TenantIcpAddress")]
    public string? IcpAddress { get; set; }

    [DynamicStringLength(typeof(TenantConsts), nameof(TenantConsts.MaxRemarkLength))]
    [Display(Name = "TenantRemark")]
    public string? Remark { get; set; }

    [Range(0, int.MaxValue)]
    public int MaxUserCount { get; set; }

    public bool IsActive { get; set; } = true;

    public TenantCreateOrUpdateDtoBase() : base(false)
    {

    }
}
