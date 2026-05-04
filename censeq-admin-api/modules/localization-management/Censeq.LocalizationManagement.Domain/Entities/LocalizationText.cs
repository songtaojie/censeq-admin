using System;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Censeq.LocalizationManagement.Entities;

/// <summary>
/// 翻译条目表（核心表）— 存储数据库级别的翻译覆盖值
/// 优先级：租户级 DB 翻译 > Host 级 DB 翻译 > JSON 文件默认值
/// </summary>
public class LocalizationText : AuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; protected set; }

    [NotNull]
    public virtual string ResourceName { get; protected set; } = null!;

    [NotNull]
    public virtual string CultureName { get; protected set; } = null!;

    [NotNull]
    public virtual string Key { get; protected set; } = null!;

    [CanBeNull]
    public virtual string? Value { get; set; }

    protected LocalizationText() { }

    public LocalizationText(
        Guid id,
        [NotNull] string resourceName,
        [NotNull] string cultureName,
        [NotNull] string key,
        string? value = null,
        Guid? tenantId = null)
    {
        Id = id;
        ResourceName = Check.NotNullOrWhiteSpace(resourceName, nameof(resourceName), LocalizationTextConsts.MaxResourceNameLength);
        CultureName = Check.NotNullOrWhiteSpace(cultureName, nameof(cultureName), LocalizationTextConsts.MaxCultureNameLength);
        Key = Check.NotNullOrWhiteSpace(key, nameof(key), LocalizationTextConsts.MaxKeyLength);
        Value = value;
        TenantId = tenantId;
    }
}
