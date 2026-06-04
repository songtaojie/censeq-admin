﻿using System.Collections.Generic;

namespace Censeq.Account.Web.ProfileManagement;

/// <summary>
/// 个人资料管理页面配置项。
/// </summary>
public class ProfileManagementPageOptions
{
    /// <summary>
    /// 贡献器列表。
    /// </summary>
    public List<IProfileManagementPageContributor> Contributors { get; }

    /// <summary>
    /// 初始化 ProfileManagementPageOptions 实例。
    /// </summary>
    public ProfileManagementPageOptions()
    {
        Contributors = new List<IProfileManagementPageContributor>();
    }
}
