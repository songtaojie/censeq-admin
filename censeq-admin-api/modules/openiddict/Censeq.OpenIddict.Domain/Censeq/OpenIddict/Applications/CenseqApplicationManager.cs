using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.Core;

namespace Censeq.OpenIddict.Applications;

/// <summary>
/// 应用程序管理器，封装领域管理逻辑。
/// </summary>
public class CenseqApplicationManager : OpenIddictApplicationManager<OpenIddictApplicationModel>, ICenseqApplicationManager
{
    /// <summary>
    /// 标识转换器。
    /// </summary>
    protected CenseqOpenIddictIdentifierConverter IdentifierConverter { get; }

    /// <summary>
    /// 初始化 CenseqApplicationManager 实例。
    /// </summary>
    /// <param name="cache">缓存。</param>
    /// <param name="logger">logger。</param>
    /// <param name="options">配置项。</param>
    /// <param name="resolver">resolver。</param>
    /// <param name="identifierConverter">dentifier转换器。</param>
    public CenseqApplicationManager(
        [NotNull] IOpenIddictApplicationCache<OpenIddictApplicationModel> cache,
        [NotNull] ILogger<CenseqApplicationManager> logger,
        [NotNull] IOptionsMonitor<OpenIddictCoreOptions> options,
        [NotNull] IOpenIddictApplicationStoreResolver resolver,
        CenseqOpenIddictIdentifierConverter identifierConverter)
        : base(cache, logger, options, resolver)
    {
        IdentifierConverter = identifierConverter;
    }

    /// <summary>
    /// 更新数据。
    /// </summary>
    /// <param name="application">OpenIddict 应用程序。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>更新后的数据。</returns>
    public async override ValueTask UpdateAsync(OpenIddictApplicationModel application, CancellationToken cancellationToken = default)
    {
        if (!Options.CurrentValue.DisableEntityCaching)
        {
            var entity = await Store.FindByIdAsync(IdentifierConverter.ToString(application.Id), cancellationToken);
            if (entity != null)
            {
                await Cache.RemoveAsync(entity, cancellationToken);
            }
        }

        await base.UpdateAsync(application, cancellationToken);
    }

    /// <summary>
    /// 异步填充数据。
    /// </summary>
    /// <param name="descriptor">描述符。</param>
    /// <param name="application">OpenIddict 应用程序。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>表示异步操作的任务。</returns>
    public async override ValueTask PopulateAsync(OpenIddictApplicationDescriptor descriptor, OpenIddictApplicationModel application, CancellationToken cancellationToken = default)
    {
        await base.PopulateAsync(descriptor, application, cancellationToken);

        if (descriptor is CenseqApplicationDescriptor model)
        {
            model.ClientUri = application.ClientUri;
            model.LogoUri = application.LogoUri;
        }
    }

    /// <summary>
    /// 异步填充数据。
    /// </summary>
    /// <param name="application">OpenIddict 应用程序。</param>
    /// <param name="descriptor">描述符。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>表示异步操作的任务。</returns>
    public async override ValueTask PopulateAsync(OpenIddictApplicationModel application, OpenIddictApplicationDescriptor descriptor, CancellationToken cancellationToken = default)
    {
        await base.PopulateAsync(application, descriptor, cancellationToken);

        if (descriptor is CenseqApplicationDescriptor model)
        {
            application.ClientUri = model.ClientUri;
            application.LogoUri = model.LogoUri;
        }
    }

    /// <summary>
    /// 获取客户端 URI。
    /// </summary>
    /// <param name="application">OpenIddict 应用程序。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    public virtual async ValueTask<string> GetClientUriAsync(object application, CancellationToken cancellationToken = default)
    {
        Check.NotNull(application, nameof(application));
        Check.AssignableTo<ICenseqOpenIdApplicationStore>(application.GetType(), nameof(application));

        return await Store.As<ICenseqOpenIdApplicationStore>().GetClientUriAsync(application.As<OpenIddictApplicationModel>(), cancellationToken);
    }

    /// <summary>
    /// 获取 Logo URI。
    /// </summary>
    /// <param name="application">OpenIddict 应用程序。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>异步操作结果。</returns>
    public virtual async ValueTask<string> GetLogoUriAsync(object application, CancellationToken cancellationToken = default)
    {
        Check.NotNull(application, nameof(application));
        Check.AssignableTo<ICenseqOpenIdApplicationStore>(application.GetType(), nameof(application));

        return await Store.As<ICenseqOpenIdApplicationStore>().GetLogoUriAsync(application.As<OpenIddictApplicationModel>(), cancellationToken);
    }
}
