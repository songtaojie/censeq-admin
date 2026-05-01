using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Censeq.TenantManagement;

public class TenantDto : ExtensibleEntityDto<Guid>, IHasConcurrencyStamp
{
    public string Name { get; set; }

    public string? Code { get; set; }

    public string? Domain { get; set; }

    public string? Icon { get; set; }

    public string? Copyright { get; set; }

    public string? IcpNo { get; set; }

    public string? IcpAddress { get; set; }

    public string? Remark { get; set; }

    public int MaxUserCount { get; set; }

    public bool IsActive { get; set; }

    public string ConcurrencyStamp { get; set; }
}
