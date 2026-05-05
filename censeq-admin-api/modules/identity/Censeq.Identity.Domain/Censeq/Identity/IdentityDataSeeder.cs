using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Censeq.PermissionManagement;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace Censeq.Identity;

/// <summary>
/// 身份Data种子数据
/// </summary>
public class IdentityDataSeeder : ITransientDependency, IIdentityDataSeeder
{
    private static readonly string[] DefaultAdminPermissions =
    [
        "CenseqAdmin.Menus",
        "CenseqAdmin.Menus.Create",
        "CenseqAdmin.Menus.Update",
        "CenseqAdmin.Menus.Delete",
        "CenseqAdmin.Menus.ManageStatus",
        "CenseqAdmin.Menus.ManageOrder",
        "CenseqAdmin.Menus.CopyFromHost",
        "SettingManagement.SettingDefinitions",
        "SettingManagement.SettingDefinitions.Create",
        "SettingManagement.SettingDefinitions.Update",
        "SettingManagement.SettingDefinitions.Delete",
        "PermissionManagement.DefinitionManagement"
    ];

    /// <summary>
    /// IGuidGenerator
    /// </summary>
    protected IGuidGenerator GuidGenerator { get; }
    /// <summary>
    /// I身份角色仓储
    /// </summary>
    protected IIdentityRoleRepository RoleRepository { get; }
    /// <summary>
    /// I身份声明类型仓储
    /// </summary>
    protected IIdentityClaimTypeRepository ClaimTypeRepository { get; }
    /// <summary>
    /// I身份用户仓储
    /// </summary>
    protected IIdentityUserRepository UserRepository { get; }
    /// <summary>
    /// I查找Normalizer
    /// </summary>
    protected ILookupNormalizer LookupNormalizer { get; }
    /// <summary>
    /// 身份用户管理器
    /// </summary>
    protected IdentityUserManager UserManager { get; }
    /// <summary>
    /// 身份角色管理器
    /// </summary>
    protected IdentityRoleManager RoleManager { get; }
    /// <summary>
    /// 身份声明类型管理器
    /// </summary>
    protected IdentityClaimTypeManager ClaimTypeManager { get; }
    /// <summary>
    /// I权限Data种子数据
    /// </summary>
    protected IPermissionDataSeeder PermissionDataSeeder { get; }
    /// <summary>
    /// ICurrent租户
    /// </summary>
    protected ICurrentTenant CurrentTenant { get; }
    /// <summary>
    /// IOptions<IdentityOptions>
    /// </summary>
    protected IOptions<IdentityOptions> IdentityOptions { get; }

    public IdentityDataSeeder(
        IGuidGenerator guidGenerator,
        IIdentityRoleRepository roleRepository,
        IIdentityClaimTypeRepository claimTypeRepository,
        IIdentityUserRepository userRepository,
        ILookupNormalizer lookupNormalizer,
        IdentityUserManager userManager,
        IdentityRoleManager roleManager,
        IdentityClaimTypeManager claimTypeManager,
        IPermissionDataSeeder permissionDataSeeder,
        ICurrentTenant currentTenant,
        IOptions<IdentityOptions> identityOptions)
    {
        GuidGenerator = guidGenerator;
        RoleRepository = roleRepository;
        ClaimTypeRepository = claimTypeRepository;
        UserRepository = userRepository;
        LookupNormalizer = lookupNormalizer;
        UserManager = userManager;
        RoleManager = roleManager;
        ClaimTypeManager = claimTypeManager;
        PermissionDataSeeder = permissionDataSeeder;
        CurrentTenant = currentTenant;
        IdentityOptions = identityOptions;
    }

    [UnitOfWork]
    /// <summary>
    /// Task<IdentityData种子Result>
    /// </summary>
    public virtual async Task<IdentityDataSeedResult> SeedAsync(
        string adminEmail,
        string adminPassword,
        Guid? tenantId = null,
        string? adminUserName = null,
        string? adminName = null)
    {
        Check.NotNullOrWhiteSpace(adminEmail, nameof(adminEmail));
        Check.NotNullOrWhiteSpace(adminPassword, nameof(adminPassword));

        using (CurrentTenant.Change(tenantId))
        {
            await IdentityOptions.SetAsync();

            var result = new IdentityDataSeedResult();
            //"admin" user
            if (adminUserName.IsNullOrWhiteSpace())
            {
                adminUserName = IdentityDataSeedContributor.AdminUserNameDefaultValue;
            }

            if (adminName.IsNullOrWhiteSpace())
            {
                adminName = adminUserName;
            }

            var adminUser = await UserRepository.FindByNormalizedUserNameAsync(
                LookupNormalizer.NormalizeName(adminUserName)
            );

            if (adminUser == null)
            {
                adminUser = new IdentityUser(
                    GuidGenerator.Create(),
                    adminUserName,
                    adminEmail,
                    tenantId
                )
                {
                    Name = adminName
                };

                (await UserManager.CreateAsync(adminUser, adminPassword, validatePassword: false)).CheckErrors();
                result.CreatedAdminUser = true;
            }

            //"admin" role
            const string adminRoleName = "admin";
            var adminRole =
                await RoleRepository.FindByNormalizedNameAsync(LookupNormalizer.NormalizeName(adminRoleName));
            if (adminRole == null)
            {
                adminRole = new IdentityRole(
                    GuidGenerator.Create(),
                    adminRoleName,
                    tenantId
                )
                {
                    IsStatic = true,
                    IsPublic = true
                };

                (await RoleManager.CreateAsync(adminRole)).CheckErrors();
                result.CreatedAdminRole = true;
            }

            await PermissionDataSeeder.SeedAsync(
                RolePermissionValueProvider.ProviderName,
                adminRoleName,
                DefaultAdminPermissions,
                tenantId
            );

            if (!await UserManager.IsInRoleAsync(adminUser, adminRoleName))
            {
                (await UserManager.AddToRoleAsync(adminUser, adminRoleName)).CheckErrors();
            }

            if (tenantId == null)
            {
                await SeedClaimTypesAsync();
            }

            return result;
        }
    }

    protected virtual async Task SeedClaimTypesAsync()
    {
        var claimTypes = new List<IdentityClaimType>
        {
            new IdentityClaimType(GuidGenerator.Create(), "DataScope", false, false, null, null, "数据范围", IdentityClaimValueType.String),
            new IdentityClaimType(GuidGenerator.Create(), "MaxAmount", false, false, null, null, "最大审批金额", IdentityClaimValueType.Int),
            new IdentityClaimType(GuidGenerator.Create(), "DepartmentId", false, false, null, null, "部门ID", IdentityClaimValueType.String)
        };

        foreach (var claimType in claimTypes)
        {
            if (await ClaimTypeRepository.AnyAsync(claimType.Name))
            {
                continue;
            }

            await ClaimTypeManager.CreateAsync(claimType);
        }
    }
}
