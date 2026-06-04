﻿using System.Threading.Tasks;

namespace Censeq.Account.Web.ProfileManagement;

/// <summary>
/// 个人资料管理页面贡献器接口。
/// </summary>
public interface IProfileManagementPageContributor
{
    /// <summary>
    /// 异步配置页面分组。
    /// </summary>
    /// <param name="context">当前上下文。</param>
    /// <returns>表示异步操作的任务。</returns>
    Task ConfigureAsync(ProfileManagementPageCreationContext context);
}
