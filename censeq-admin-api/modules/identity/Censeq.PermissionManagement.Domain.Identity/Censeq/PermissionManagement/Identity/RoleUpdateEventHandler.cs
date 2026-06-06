using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Censeq.Identity;

namespace Censeq.PermissionManagement.Identity;

/// <summary>
/// 角色更新事件处理器
/// </summary>
public class RoleUpdateEventHandler :
    IDistributedEventHandler<IdentityRoleNameChangedEto>,
    ITransientDependency
{
    public async Task HandleEventAsync(IdentityRoleNameChangedEto eventData)
    {
        await Task.CompletedTask;
    }
}
