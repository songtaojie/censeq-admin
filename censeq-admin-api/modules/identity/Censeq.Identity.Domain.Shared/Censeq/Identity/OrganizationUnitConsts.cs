namespace Censeq.Identity;

/// <summary>
/// 组织单元常量定义
/// </summary>
public static class OrganizationUnitConsts
{
    /// <summary>
    /// 显示名称最大长度，默认值：128
    /// </summary>
    public static int MaxDisplayNameLength { get; set; } = 128;

    /// <summary>
    /// 组织单元层级最大深度
    /// </summary>
    public const int MaxDepth = 16;

    /// <summary>
    /// 点分隔的代码单元长度
    /// </summary>
    public const int CodeUnitLength = 5;

    /// <summary>
    /// 代码属性最大长度
    /// </summary>
    public const int MaxCodeLength = MaxDepth * (CodeUnitLength + 1) - 1;
}
