using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace Censeq.PermissionManagement;

/// <summary>
/// 权限管理数据种子贡献器。
/// 角色授权已改为使用角色 ID 作为 ProviderKey，默认角色授权由身份模块在创建默认管理员角色后写入。
/// </summary>
public class PermissionDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    /// <summary>
    /// 执行权限管理模块种子数据初始化。
    /// 当前模块不再写入硬编码角色授权，避免产生基于角色名称的授权记录。
    /// </summary>
    /// <param name="context">数据种子上下文。</param>
    /// <returns>异步任务。</returns>
    public virtual Task SeedAsync(DataSeedContext context)
    {
        return Task.CompletedTask;
    }
}
