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
        "PermissionManagement.DefinitionManagement",
        "AuditLogging.AuditLogs"
    ];

    protected IGuidGenerator GuidGenerator { get; }
    protected IIdentityRoleRepository RoleRepository { get; }
    protected IIdentityClaimTypeRepository ClaimTypeRepository { get; }
    protected IIdentityUserRepository UserRepository { get; }
    protected ILookupNormalizer LookupNormalizer { get; }
    protected IdentityUserManager UserManager { get; }
    protected IdentityRoleManager RoleManager { get; }
    protected IdentityClaimTypeManager ClaimTypeManager { get; }
    protected IPermissionDataSeeder PermissionDataSeeder { get; }
    protected ICurrentTenant CurrentTenant { get; }
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
    public virtual async Task<IdentityDataSeedResult> SeedAsync(
        string adminEmail,
        string adminPassword,
        Guid? tenantId = null,
        string? adminUserName = null)
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
                    Name = adminUserName
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
