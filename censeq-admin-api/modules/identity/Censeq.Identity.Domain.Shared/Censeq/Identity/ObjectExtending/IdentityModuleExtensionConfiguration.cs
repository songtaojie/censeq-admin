using System;
using Volo.Abp.ObjectExtending.Modularity;

namespace Censeq.Identity.ObjectExtending;

/// <summary>
/// 身份模块扩展配置
/// </summary>
public class IdentityModuleExtensionConfiguration : ModuleExtensionConfiguration
{
    /// <summary>
    /// 配置用户实体扩展
    /// </summary>
    /// <param name="configureAction">配置操作</param>
    /// <returns>身份模块扩展配置</returns>
    /// <summary>
    /// 身份模块Extension配置
    /// </summary>
    public IdentityModuleExtensionConfiguration ConfigureUser(
        Action<EntityExtensionConfiguration> configureAction)
    {
        return this.ConfigureEntity(
            IdentityModuleExtensionConsts.EntityNames.User,
            configureAction
        );
    }

    /// <summary>
    /// 配置角色实体扩展
    /// </summary>
    /// <param name="configureAction">配置操作</param>
    /// <returns>身份模块扩展配置</returns>
    /// <summary>
    /// 身份模块Extension配置
    /// </summary>
    public IdentityModuleExtensionConfiguration ConfigureRole(
        Action<EntityExtensionConfiguration> configureAction)
    {
        return this.ConfigureEntity(
            IdentityModuleExtensionConsts.EntityNames.Role,
            configureAction
        );
    }

    /// <summary>
    /// 配置声明类型实体扩展
    /// </summary>
    /// <param name="configureAction">配置操作</param>
    /// <returns>身份模块扩展配置</returns>
    /// <summary>
    /// 身份模块Extension配置
    /// </summary>
    public IdentityModuleExtensionConfiguration ConfigureClaimType(
        Action<EntityExtensionConfiguration> configureAction)
    {
        return this.ConfigureEntity(
            IdentityModuleExtensionConsts.EntityNames.ClaimType,
            configureAction
        );
    }

    /// <summary>
    /// 配置组织单元实体扩展
    /// </summary>
    /// <param name="configureAction">配置操作</param>
    /// <returns>身份模块扩展配置</returns>
    /// <summary>
    /// 身份模块Extension配置
    /// </summary>
    public IdentityModuleExtensionConfiguration ConfigureOrganizationUnit(
        Action<EntityExtensionConfiguration> configureAction)
    {
        return this.ConfigureEntity(
            IdentityModuleExtensionConsts.EntityNames.OrganizationUnit,
            configureAction
        );
    }
}
