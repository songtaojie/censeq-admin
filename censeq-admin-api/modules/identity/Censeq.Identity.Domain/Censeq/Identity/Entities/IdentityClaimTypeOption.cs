using System;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace Censeq.Identity.Entities;

/// <summary>
/// 身份声明类型下拉选项
/// </summary>
public class IdentityClaimTypeOption : Entity<Guid>
{
    public virtual Guid ClaimTypeId { get; protected set; }

    public virtual string Label { get; protected set; }

    public virtual string Value { get; protected set; }

    public virtual int Sort { get; set; }

    public virtual bool IsEnabled { get; set; }

    protected IdentityClaimTypeOption()
    {
        Label = string.Empty;
        Value = string.Empty;
    }

    public IdentityClaimTypeOption(
        Guid id,
        Guid claimTypeId,
        [NotNull] string label,
        [NotNull] string value,
        int sort = 0,
        bool isEnabled = true)
    {
        Id = id;
        ClaimTypeId = claimTypeId;
        SetLabel(label);
        SetValue(value);
        Sort = sort;
        IsEnabled = isEnabled;
    }

    public void SetLabel([NotNull] string label)
    {
        Label = Check.NotNullOrWhiteSpace(label, nameof(label));
    }

    public void SetValue([NotNull] string value)
    {
        Value = Check.NotNullOrWhiteSpace(value, nameof(value));
    }
}
