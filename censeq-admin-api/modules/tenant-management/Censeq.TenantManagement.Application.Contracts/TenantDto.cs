using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Censeq.TenantManagement;

public class TenantDto : ExtensibleEntityDto<Guid>, IHasConcurrencyStamp
{
    public string Name { get; set; }

    public string? Code { get; set; }

    public bool IsActive { get; set; }

    public string ConcurrencyStamp { get; set; }
}
