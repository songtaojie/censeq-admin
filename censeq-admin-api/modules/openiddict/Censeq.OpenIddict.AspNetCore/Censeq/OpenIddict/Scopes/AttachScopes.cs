using System;
using System.Linq;
using System.Threading.Tasks;
using OpenIddict.Server;

namespace Censeq.OpenIddict.Scopes;

/// <summary>
/// Attach作用域。
/// </summary>
public class AttachScopes : IOpenIddictServerHandler<OpenIddictServerEvents.HandleConfigurationRequestContext>
{
    /// <summary>
    /// 描述符。
    /// </summary>
    public static OpenIddictServerHandlerDescriptor Descriptor { get; }
        = OpenIddictServerHandlerDescriptor.CreateBuilder<OpenIddictServerEvents.HandleConfigurationRequestContext>()
            .UseSingletonHandler<AttachScopes>()
            .SetOrder(OpenIddictServerHandlers.Discovery.AttachScopes.Descriptor.Order + 1)
            .SetType(OpenIddictServerHandlerType.Custom)
            .Build();

    /// <summary>
    /// OpenIddict 作用域仓储。
    /// </summary>
    private readonly IOpenIddictScopeRepository _scopeRepository;

    /// <summary>
    /// 初始化 AttachScopes 实例。
    /// </summary>
    /// <param name="scopeRepository">作用域仓储。</param>
    public AttachScopes(IOpenIddictScopeRepository scopeRepository)
    {
        _scopeRepository = scopeRepository;
    }

    /// <summary>
    /// 处理当前请求。
    /// </summary>
    /// <param name="context">当前上下文。</param>
    /// <returns>表示异步操作的任务。</returns>
    public virtual async ValueTask HandleAsync(OpenIddictServerEvents.HandleConfigurationRequestContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var scopes = await _scopeRepository.GetListAsync();
        context.Scopes.UnionWith(scopes.Select(x => x.Name));
    }
}
