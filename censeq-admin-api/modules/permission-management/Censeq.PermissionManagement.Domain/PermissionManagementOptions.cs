using System.Collections.Generic;
using Volo.Abp.Collections;

namespace Censeq.PermissionManagement;

/// <summary>
/// 权限管理模块配置项。
/// </summary>
public class PermissionManagementOptions
{
    /// <summary>
    /// 权限管理提供者类型列表。
    /// </summary>
    public ITypeList<IPermissionManagementProvider> ManagementProviders { get; }
    /// <summary>
    /// 权限提供者对应的授权策略配置。
    /// </summary>
    public Dictionary<string, string> ProviderPolicies { get; }

    /// <summary>
    /// 是否将静态权限定义保存到数据库。
    /// </summary>
    public bool SaveStaticPermissionsToDatabase { get; set; } = true;
    /// <summary>
    /// 是否启用动态权限存储。
    /// </summary>
    public bool IsDynamicPermissionStoreEnabled { get; set; }

    /// <summary>
    /// 初始化 PermissionManagementOptions 实例。
    /// </summary>
    public PermissionManagementOptions()
    {
        ManagementProviders = new TypeList<IPermissionManagementProvider>();
        ProviderPolicies = [];
    }
}
