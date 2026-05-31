using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.ObjectExtending;

namespace Censeq.Identity;

/// <summary>
/// 身份声明类型数据传输对象
/// </summary>
public class IdentityClaimTypeDto : ExtensibleEntityDto<Guid>
{
    public string Name { get; set; }

    public bool Required { get; set; }

    public bool IsStatic { get; set; }

    public string? Regex { get; set; }

    public string? RegexDescription { get; set; }

    public string? Description { get; set; }

    public string ValueType { get; set; }

    public List<IdentityClaimTypeOptionDto> Options { get; set; } = new();
}

public class IdentityClaimTypeOptionDto : EntityDto<Guid>
{
    public string Label { get; set; }

    public string Value { get; set; }

    public int Sort { get; set; }

    public bool IsEnabled { get; set; }
}

/// <summary>
/// 身份声明类型创建数据传输对象
/// </summary>
public class IdentityClaimTypeCreateDto : ExtensibleObject
{
    public string Name { get; set; }

    public bool Required { get; set; }

    public bool IsStatic { get; set; }

    public string? Regex { get; set; }

    public string? RegexDescription { get; set; }

    public string? Description { get; set; }

    public string ValueType { get; set; }

    public List<IdentityClaimTypeOptionCreateOrUpdateDto> Options { get; set; } = new();
}

/// <summary>
/// 身份声明类型更新数据传输对象
/// </summary>
public class IdentityClaimTypeUpdateDto : ExtensibleObject
{
    public string Name { get; set; }

    public bool Required { get; set; }

    public string? Regex { get; set; }

    public string? RegexDescription { get; set; }

    public string? Description { get; set; }

    public string ValueType { get; set; }

    public List<IdentityClaimTypeOptionCreateOrUpdateDto> Options { get; set; } = new();
}

public class IdentityClaimTypeOptionCreateOrUpdateDto
{
    public string Label { get; set; }

    public string Value { get; set; }

    public int Sort { get; set; }

    public bool IsEnabled { get; set; } = true;
}
