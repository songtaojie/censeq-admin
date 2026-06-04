using System;
using System.Collections.Generic;

namespace Censeq.Account.Web.ProfileManagement;

/// <summary>
/// 个人资料管理页面创建上下文。
/// </summary>
public class ProfileManagementPageCreationContext
{
    /// <summary>
    /// 服务提供者。
    /// </summary>
    public IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// 分组集合。
    /// </summary>
    public List<ProfileManagementPageGroup> Groups { get; }

    /// <summary>
    /// 初始化 ProfileManagementPageCreationContext 实例。
    /// </summary>
    /// <param name="serviceProvider">服务提供者。</param>
    public ProfileManagementPageCreationContext(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;

        Groups = new List<ProfileManagementPageGroup>();
    }
}
