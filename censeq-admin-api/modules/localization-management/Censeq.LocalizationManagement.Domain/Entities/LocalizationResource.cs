using System;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Censeq.LocalizationManagement.Entities;

/// <summary>
/// 本地化资源表 — 记录系统中所有 ABP Resource 名称（Host 级，无多租户）
/// </summary>
public class LocalizationResource : AuditedAggregateRoot<Guid>
{
    [NotNull]
    public virtual string Name { get; protected set; } = null!;

    [CanBeNull]
    public virtual string? DisplayName { get; set; }

    [CanBeNull]
    public virtual string? DefaultCultureName { get; set; }

    protected LocalizationResource() { }

    public LocalizationResource(Guid id, [NotNull] string name, string? displayName = null, string? defaultCultureName = null)
    {
        Id = id;
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), LocalizationResourceConsts.MaxNameLength);
        DisplayName = displayName;
        DefaultCultureName = defaultCultureName;
    }
}
