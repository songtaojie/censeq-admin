using System;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Censeq.LocalizationManagement.Entities;

/// <summary>
/// 语言表 — 每个租户可独立控制启用哪些语言
/// </summary>
public class LocalizationCulture : AuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; protected set; }

    [NotNull]
    public virtual string CultureName { get; protected set; } = null!;

    /// <summary>
    /// UI 文化名，通常与 CultureName 相同，保留备用
    /// </summary>
    [CanBeNull]
    public virtual string? UiCultureName { get; set; }

    [NotNull]
    public virtual string DisplayName { get; set; } = null!;

    public virtual bool IsEnabled { get; set; }

    protected LocalizationCulture() { }

    public LocalizationCulture(
        Guid id,
        [NotNull] string cultureName,
        [NotNull] string displayName,
        bool isEnabled = true,
        string? uiCultureName = null,
        Guid? tenantId = null)
    {
        Id = id;
        CultureName = Check.NotNullOrWhiteSpace(cultureName, nameof(cultureName), LocalizationCultureConsts.MaxCultureNameLength);
        DisplayName = Check.NotNullOrWhiteSpace(displayName, nameof(displayName), LocalizationCultureConsts.MaxDisplayNameLength);
        UiCultureName = uiCultureName;
        IsEnabled = isEnabled;
        TenantId = tenantId;
    }
}
