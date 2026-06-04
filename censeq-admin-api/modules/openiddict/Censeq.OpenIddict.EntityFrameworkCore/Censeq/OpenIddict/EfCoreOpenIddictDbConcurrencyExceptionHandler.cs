using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace Censeq.OpenIddict;

/// <summary>
/// EF CoreOpenIddict 数据库并发异常处理器。
/// </summary>
public class EfCoreOpenIddictDbConcurrencyExceptionHandler : IOpenIddictDbConcurrencyExceptionHandler, ITransientDependency
{
    /// <summary>
    /// 处理当前请求。
    /// </summary>
    /// <param name="exception">异常。</param>
    /// <returns>表示异步操作的任务。</returns>
    public virtual Task HandleAsync(AbpDbConcurrencyException exception)
    {
        if (exception != null &&
            exception.InnerException is DbUpdateConcurrencyException updateConcurrencyException)
        {
            foreach (var entry in updateConcurrencyException.Entries)
            {
                // Reset the state of the entity to prevents future calls to SaveChangesAsync() from failing.
                entry.State = EntityState.Unchanged;
            }
        }

        return Task.CompletedTask;
    }
}
