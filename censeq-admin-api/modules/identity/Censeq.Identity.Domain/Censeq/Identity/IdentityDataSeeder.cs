using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Censeq.Admin.Permissions;
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
    public const string AdminRoleCode = "admin";
    public const string AdminRoleName = "管理员";
    public const string HostAdminDisplayName = "平台管理员";
    public const string TenantAdminDisplayName = "管理员";

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
            adminName = tenantId == null ? HostAdminDisplayName : TenantAdminDisplayName;

            //"admin" user
            if (adminUserName.IsNullOrWhiteSpace())
            {
                adminUserName = IdentityDataSeedContributor.AdminUserNameDefaultValue;
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
            else if (!string.Equals(adminUser.Name, adminName, StringComparison.Ordinal))
            {
                adminUser.Name = adminName;
                (await UserManager.UpdateAsync(adminUser)).CheckErrors();
            }

            // 管理员角色
            var adminRole = await RoleRepository.FindByCodeAsync(AdminRoleCode,false);

            if (adminRole == null)
            {
                adminRole = new IdentityRole(
                    GuidGenerator.Create(),
                    AdminRoleName,
                    tenantId
                )
                {
                    IsStatic = true,
                    IsPublic = true
                };

                adminRole.Code = AdminRoleCode;

                (await RoleManager.CreateAsync(adminRole)).CheckErrors();
                result.CreatedAdminRole = true;
            }
            else
            {
                var roleChanged = false;

                if (!string.Equals(adminRole.Name, AdminRoleName, StringComparison.Ordinal))
                {
                    adminRole.Name = AdminRoleName;
                    adminRole.NormalizedName = LookupNormalizer.NormalizeName(AdminRoleName);
                    roleChanged = true;
                }

                if (!string.Equals(adminRole.Code, AdminRoleCode, StringComparison.Ordinal))
                {
                    adminRole.Code = AdminRoleCode;
                    roleChanged = true;
                }

                if (!adminRole.IsStatic)
                {
                    adminRole.IsStatic = true;
                    roleChanged = true;
                }

                if (!adminRole.IsPublic)
                {
                    adminRole.IsPublic = true;
                    roleChanged = true;
                }

                if (adminRole.CreationTime == default)
                {
                    adminRole.CreationTime = DateTime.Now;
                    roleChanged = true;
                }

                if (roleChanged)
                {
                    (await RoleManager.UpdateAsync(adminRole)).CheckErrors();
                }
            }

            var defaultAdminPermissions = tenantId == null
                ? AdminSeedPermissionNames.HostAdminDefaults
                : AdminSeedPermissionNames.TenantAdminDefaults;

            await PermissionDataSeeder.SeedAsync(
                RolePermissionValueProvider.ProviderName,
                AdminRoleName,
                defaultAdminPermissions,
                tenantId
            );

            if (!await UserManager.IsInRoleAsync(adminUser, AdminRoleName))
            {
                (await UserManager.AddToRoleAsync(adminUser, AdminRoleName)).CheckErrors();
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
