using System;

namespace Censeq.Identity;

/// <summary>
/// 身份声明类型事件传输对象（ETO）
/// </summary>
[Serializable]
/// <summary>
/// 身份声明类型事件传输对象
/// </summary>
public class IdentityClaimTypeEto
{
    /// <summary>
    /// 标识
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 是否必填
    /// </summary>
    public bool Required { get; set; }

    /// <summary>
    /// 是否为静态声明类型
    /// </summary>
    public bool IsStatic { get; set; }

    /// <summary>
    /// 正则表达式
    /// </summary>
    public string Regex { get; set; }

    /// <summary>
    /// 正则表达式描述
    /// </summary>
    public string RegexDescription { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 值类型
    /// </summary>
    public IdentityClaimValueType ValueType { get; set; }
}
