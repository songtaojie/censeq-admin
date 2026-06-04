using System.Collections.Generic;
using Volo.Abp;

namespace Censeq.PermissionManagement;

/// <summary>
/// 多个权限在同一权限值提供者下的授予信息。
/// </summary>
public class MultiplePermissionValueProviderGrantInfo
{
    /// <summary>
    /// 按权限名称索引的授权结果。
    /// </summary>
    public Dictionary<string, PermissionValueProviderGrantInfo> Result { get; }

    /// <summary>
    /// 初始化 MultiplePermissionValueProviderGrantInfo 实例。
    /// </summary>
    public MultiplePermissionValueProviderGrantInfo()
    {
        Result = [];
    }

    /// <summary>
    /// 初始化 MultiplePermissionValueProviderGrantInfo 实例。
    /// </summary>
    /// <param name="names">names。</param>
    public MultiplePermissionValueProviderGrantInfo(string[] names)
    {
        Check.NotNull(names, nameof(names));
        Result = new Dictionary<string, PermissionValueProviderGrantInfo>(names.Length);
        foreach (var name in names)
            Result.Add(name, PermissionValueProviderGrantInfo.NonGranted);
    }
}
