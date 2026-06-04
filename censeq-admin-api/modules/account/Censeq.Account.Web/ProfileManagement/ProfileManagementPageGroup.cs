using System;
using JetBrains.Annotations;

namespace Censeq.Account.Web.ProfileManagement;

/// <summary>
/// 个人资料管理页面分组。
/// </summary>
public class ProfileManagementPageGroup
{
    /// <summary>
    /// 标识。
    /// </summary>
    public string Id {  get; set; }

    /// <summary>
    /// 显示名称。
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// 组件类型。
    /// </summary>
    public Type ComponentType { get; set; }

    /// <summary>
    /// 参数。
    /// </summary>
    public object? Parameter { get; set; }

    /// <summary>
    /// 初始化 ProfileManagementPageGroup 实例。
    /// </summary>
    /// <param name="id">标识。</param>
    /// <param name="displayName">显示名称。</param>
    /// <param name="componentType">组件类型。</param>
    /// <param name="parameter">parameter。</param>
    public ProfileManagementPageGroup(string id, string displayName, Type componentType, object? parameter = null)
    {
        Id = id;
        DisplayName = displayName;
        ComponentType = componentType;
        Parameter = parameter;
    }
}
