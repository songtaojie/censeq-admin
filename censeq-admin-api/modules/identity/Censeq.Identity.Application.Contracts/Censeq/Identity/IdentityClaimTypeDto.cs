using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.ObjectExtending;

namespace Censeq.Identity;

public class IdentityClaimTypeDto : ExtensibleEntityDto<Guid>
{
    public string Name { get; set; }

    public bool Required { get; set; }

    public bool IsStatic { get; set; }

    public string? Regex { get; set; }

    public string? RegexDescription { get; set; }

    public string? Description { get; set; }

    public string ValueType { get; set; }
}

public class IdentityClaimTypeCreateDto : ExtensibleObject
{
    public string Name { get; set; }

    public bool Required { get; set; }

    public bool IsStatic { get; set; }

    public string? Regex { get; set; }

    public string? RegexDescription { get; set; }

    public string? Description { get; set; }

    public string ValueType { get; set; }
}

public class IdentityClaimTypeUpdateDto : ExtensibleObject
{
    public string Name { get; set; }

    public bool Required { get; set; }

    public string? Regex { get; set; }

    public string? RegexDescription { get; set; }

    public string? Description { get; set; }

    public string ValueType { get; set; }
}