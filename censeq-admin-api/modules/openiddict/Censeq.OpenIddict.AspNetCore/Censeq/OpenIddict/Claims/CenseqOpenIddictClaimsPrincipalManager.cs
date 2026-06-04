using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using Volo.Abp.DependencyInjection;

namespace Censeq.OpenIddict;

/// <summary>
/// OpenIddict 声明主体管理器，封装领域管理逻辑。
/// </summary>
public class CenseqOpenIddictClaimsPrincipalManager : ISingletonDependency
{
    /// <summary>
    /// 服务作用域工厂。
    /// </summary>
    protected IServiceScopeFactory ServiceScopeFactory { get; }
    /// <summary>
    /// 配置项。
    /// </summary>
    protected IOptions<CenseqOpenIddictClaimsPrincipalOptions> Options { get; }

    /// <summary>
    /// 初始化 CenseqOpenIddictClaimsPrincipalManager 实例。
    /// </summary>
    /// <param name="serviceScopeFactory">服务作用域工厂。</param>
    /// <param name="options">配置项。</param>
    public CenseqOpenIddictClaimsPrincipalManager(IServiceScopeFactory serviceScopeFactory, IOptions<CenseqOpenIddictClaimsPrincipalOptions> options)
    {
        ServiceScopeFactory = serviceScopeFactory;
        Options = options;
    }

    /// <summary>
    /// 处理当前请求。
    /// </summary>
    /// <param name="openIddictRequest">OpenIddict 请求。</param>
    /// <param name="principal">主体。</param>
    /// <returns>表示异步操作的任务。</returns>
    public virtual async Task HandleAsync(OpenIddictRequest openIddictRequest, ClaimsPrincipal principal)
    {
        using (var scope = ServiceScopeFactory.CreateScope())
        {
            foreach (var providerType in Options.Value.ClaimsPrincipalHandlers)
            {
                var provider = (ICenseqOpenIddictClaimsPrincipalHandler)scope.ServiceProvider.GetRequiredService(providerType);
                await provider.HandleAsync(new CenseqOpenIddictClaimsPrincipalHandlerContext(scope.ServiceProvider, openIddictRequest, principal));
            }
        }
    }
}
