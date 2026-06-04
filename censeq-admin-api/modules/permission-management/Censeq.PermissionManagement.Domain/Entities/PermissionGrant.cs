using System;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Censeq.PermissionManagement.Entities;

/// <summary>
/// 权限授予持久化记录。
/// </summary>
public class PermissionGrant : BasicAggregateRoot<Guid>, IMultiTenant
{
    /// <summary>
    /// 租户标识。
    /// </summary>
    public virtual Guid? TenantId { get; protected set; }
    /// <summary>
    /// 权限名称。
    /// </summary>
    public virtual string Name { get; protected set; } = string.Empty;
    /// <summary>
    /// 权限提供者名称。
    /// </summary>
    public virtual string ProviderName { get; protected set; } = string.Empty;
    /// <summary>
    /// 权限提供者标识。
    /// </summary>
    public virtual string ProviderKey { get; protected internal set; } = string.Empty;

    /// <summary>
    /// 初始化 PermissionGrant 实例。
    /// </summary>
    protected PermissionGrant()
    {
    }

    /// <summary>
    /// 初始化 PermissionGrant 实例。
    /// </summary>
    /// <param name="id">实体标识。</param>
    /// <param name="name">权限名称。</param>
    /// <param name="providerName">权限提供者名称。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    /// <param name="tenantId">租户标识。</param>
    public PermissionGrant(Guid id, [NotNull] string name, [NotNull] string providerName, string providerKey, Guid? tenantId = null)
    {
        Id = id;
        Name = Check.NotNullOrWhiteSpace(name, nameof(name));
        ProviderName = Check.NotNullOrWhiteSpace(providerName, nameof(providerName));
        ProviderKey = providerKey ?? string.Empty;
        TenantId = tenantId;
    }
}
