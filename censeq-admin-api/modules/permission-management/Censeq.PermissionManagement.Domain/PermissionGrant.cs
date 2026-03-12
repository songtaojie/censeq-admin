using System;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Censeq.PermissionManagement;

public class PermissionGrant : BasicAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; protected set; }
    public virtual string Name { get; protected set; } = string.Empty;
    public virtual string ProviderName { get; protected set; } = string.Empty;
    public virtual string ProviderKey { get; protected internal set; } = string.Empty;

    protected PermissionGrant()
    {
    }

    public PermissionGrant(Guid id, [NotNull] string name, [NotNull] string providerName, string providerKey, Guid? tenantId = null)
    {
        Id = id;
        Name = Check.NotNullOrWhiteSpace(name, nameof(name));
        ProviderName = Check.NotNullOrWhiteSpace(providerName, nameof(providerName));
        ProviderKey = providerKey ?? string.Empty;
        TenantId = tenantId;
    }
}
