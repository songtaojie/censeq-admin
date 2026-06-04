using System.Threading.Tasks;

namespace Censeq.PermissionManagement;

/// <summary>
/// 静态权限定义保存器接口。
/// </summary>
public interface IStaticPermissionSaver
{
    /// <summary>
    /// 保存静态权限定义。
    /// </summary>
    /// <returns>表示异步操作的任务。</returns>
    Task SaveAsync();
}
