using System;
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
}