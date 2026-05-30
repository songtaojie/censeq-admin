using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Censeq.PermissionManagement;
/// <summary>
/// 权限种子数据服务。
/// </summary>
public interface IPermissionDataSeeder
{
    /// <summary>
    /// 初始化指定提供者的权限授予数据。
    /// </summary>
    /// <param name="providerName">权限提供者名称。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    /// <param name="grantedPermissions">需要授予的权限名称集合。</param>
    /// <param name="tenantId">租户标识，宿主侧为空。</param>
    /// <returns>异步任务。</returns>
    Task SeedAsync(string providerName,string providerKey,IEnumerable<string> grantedPermissions,Guid? tenantId = null);
}
