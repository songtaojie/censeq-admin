using System;
using Volo.Abp.ObjectExtending.Modularity;

namespace Volo.Abp.ObjectExtending;

/// <summary>
/// OpenIddict 模块扩展配置。
/// </summary>
public class OpenIddictModuleExtensionConfiguration : ModuleExtensionConfiguration
{
    /// <summary>
    /// 配置应用程序。
    /// </summary>
    /// <param name="configureAction">configure操作。</param>
    /// <returns>操作结果。</returns>
    public OpenIddictModuleExtensionConfiguration ConfigureApplication(
        Action<EntityExtensionConfiguration> configureAction)
    {
        return this.ConfigureEntity(
            OpenIddictModuleExtensionConsts.EntityNames.Application,
            configureAction
        );
    }

    /// <summary>
    /// 配置授权。
    /// </summary>
    /// <param name="configureAction">configure操作。</param>
    /// <returns>操作结果。</returns>
    public OpenIddictModuleExtensionConfiguration ConfigureAuthorization(
        Action<EntityExtensionConfiguration> configureAction)
    {
        return this.ConfigureEntity(
            OpenIddictModuleExtensionConsts.EntityNames.Authorization,
            configureAction
        );
    }

    /// <summary>
    /// 配置作用域。
    /// </summary>
    /// <param name="configureAction">configure操作。</param>
    /// <returns>操作结果。</returns>
    public OpenIddictModuleExtensionConfiguration ConfigureScope(
        Action<EntityExtensionConfiguration> configureAction)
    {
        return this.ConfigureEntity(
            OpenIddictModuleExtensionConsts.EntityNames.Scope,
            configureAction
        );
    }

    /// <summary>
    /// 配置令牌。
    /// </summary>
    /// <param name="configureAction">configure操作。</param>
    /// <returns>操作结果。</returns>
    public OpenIddictModuleExtensionConfiguration ConfigureToken(
        Action<EntityExtensionConfiguration> configureAction)
    {
        return this.ConfigureEntity(
            OpenIddictModuleExtensionConsts.EntityNames.Token,
            configureAction
        );
    }
}
