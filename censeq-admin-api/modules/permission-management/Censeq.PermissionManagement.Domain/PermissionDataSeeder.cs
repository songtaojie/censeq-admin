using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Censeq.PermissionManagement.Entities;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;

namespace Censeq.PermissionManagement;
/// <summary>
/// 权限种子数据服务。
/// 负责在指定租户上下文中补齐缺失的权限授予记录。
/// </summary>
public class PermissionDataSeeder : IPermissionDataSeeder, ITransientDependency
{
    /// <summary>
    /// 权限仓储
    /// </summary>
    protected IPermissionGrantRepository PermissionGrantRepository { get; }
    /// <summary>
    /// Guid生成器
    /// </summary>
    protected IGuidGenerator GuidGenerator { get; }

    /// <summary>
    /// 当前租户
    /// </summary>
    protected ICurrentTenant CurrentTenant { get; }

    /// <summary>
    /// 初始化权限种子数据服务。
    /// </summary>
    /// <param name="permissionGrantRepository">权限授予仓储。</param>
    /// <param name="guidGenerator">Guid 生成器。</param>
    /// <param name="currentTenant">当前租户上下文。</param>
    public PermissionDataSeeder(
        IPermissionGrantRepository permissionGrantRepository,
        IGuidGenerator guidGenerator,
        ICurrentTenant currentTenant)
    {
        PermissionGrantRepository = permissionGrantRepository;
        GuidGenerator = guidGenerator;
        CurrentTenant = currentTenant;
    }

    /// <summary>
    /// 初始化指定提供者的权限授予数据。
    /// 已存在的授权记录会被跳过，避免重复插入。
    /// </summary>
    /// <param name="providerName">权限提供者名称。</param>
    /// <param name="providerKey">权限提供者标识。</param>
    /// <param name="grantedPermissions">需要授予的权限名称集合。</param>
    /// <param name="tenantId">租户标识，宿主侧为空。</param>
    /// <returns>异步任务。</returns>
    public virtual async Task SeedAsync(string providerName,string providerKey,IEnumerable<string> grantedPermissions,Guid? tenantId = null)
    {
        using (CurrentTenant.Change(tenantId))
        {
            var names = grantedPermissions.ToArray();
            var existsPermissionGrants = (await PermissionGrantRepository.GetListAsync(names, providerName, providerKey)).Select(x => x.Name).ToList();

            foreach (var permissionName in names.Except(existsPermissionGrants))
            {
                await PermissionGrantRepository.InsertAsync(
                    new PermissionGrant(
                        GuidGenerator.Create(),
                        permissionName,
                        providerName,
                        providerKey,
                        tenantId
                    )
                );
            }
        }
    }
}
