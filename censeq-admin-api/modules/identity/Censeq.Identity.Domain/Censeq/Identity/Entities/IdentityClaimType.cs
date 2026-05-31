using Censeq.Identity;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Volo.Abp.Domain.Entities;

namespace Censeq.Identity.Entities;

/// <summary>
/// 身份声明类型
/// </summary>
public class IdentityClaimType : AggregateRoot<Guid>
{
    public virtual string Name { get; protected set; }

    public virtual bool Required { get; set; }

    public virtual bool IsStatic { get; protected set; }

    public virtual string? Regex { get; set; }

    public virtual string? RegexDescription { get; set; }

    public virtual string? Description { get; set; }

    /// <summary>
    /// 身份声明值类型
    /// </summary>
    public virtual IdentityClaimValueType ValueType { get; set; }

    public virtual ICollection<IdentityClaimTypeOption> Options { get; protected set; }

    protected IdentityClaimType()
    {
        Options = new Collection<IdentityClaimTypeOption>();
    }

    public IdentityClaimType(
        Guid id,
        [NotNull] string name,
        bool required = false,
        bool isStatic = false,
        [CanBeNull] string? regex = null,
        [CanBeNull] string? regexDescription = null,
        [CanBeNull] string? description = null,
        IdentityClaimValueType valueType = IdentityClaimValueType.String)
    {
        Id = id;
        SetName(name);
        Required = required;
        IsStatic = isStatic;
        Regex = regex;
        RegexDescription = regexDescription;
        Description = description;
        ValueType = valueType;
        Options = new Collection<IdentityClaimTypeOption>();
    }

    public void SetName([NotNull] string name)
    {
        Name = Check.NotNull(name, nameof(name));
    }

    public void SetOptions(IEnumerable<IdentityClaimTypeOption> options)
    {
        Options.Clear();
        foreach (var option in options.OrderBy(x => x.Sort))
        {
            Options.Add(option);
        }
    }
}
