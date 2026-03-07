using Microsoft.EntityFrameworkCore;
using System;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.Identity;

namespace Starshine.Admin.EntityFrameworkCore.Modeling
{
    internal static class StarshineIdentityDbContextModelBuilderExtensions
    {
        public static void ConfigureIdentity(this ModelBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            builder.Entity<IdentityUser>(b =>
            {
                b.ToStarshineTable(nameof(IdentityUser)).ConfigureStarshineByConvention();

                b.ConfigureStarshineUser();
                b.Property(u => u.NormalizedUserName).IsRequired()
                    .HasMaxLength(IdentityUserConsts.MaxNormalizedUserNameLength);
                b.Property(u => u.NormalizedEmail).IsRequired()
                    .HasMaxLength(IdentityUserConsts.MaxNormalizedEmailLength);
                b.Property(u => u.PasswordHash).HasMaxLength(IdentityUserConsts.MaxPasswordHashLength);
                b.Property(u => u.SecurityStamp).IsRequired().HasMaxLength(IdentityUserConsts.MaxSecurityStampLength);
                b.Property(u => u.TwoFactorEnabled).HasDefaultValue(false);
                b.Property(u => u.LockoutEnabled).HasDefaultValue(false);

                b.Property(u => u.IsExternal).IsRequired().HasDefaultValue(false);

                b.Property(u => u.AccessFailedCount)
                    .If(!builder.IsUsingOracle(), p => p.HasDefaultValue(0));

                b.HasIndex(u => u.NormalizedUserName);
                b.HasIndex(u => u.NormalizedEmail);
                b.HasIndex(u => u.UserName);
                b.HasIndex(u => u.Email);
                b.ApplyObjectExtensionMappings();
            });

            builder.Entity<IdentityUserClaim>(b =>
            {
                b.ToStarshineTable(nameof(IdentityUserClaim))
                    .ConfigureStarshineByConvention();

                b.Property(x => x.Id).ValueGeneratedNever();
                b.Property(uc => uc.ClaimType).HasMaxLength(IdentityUserClaimConsts.MaxClaimTypeLength).IsRequired();
                b.Property(uc => uc.ClaimValue).HasMaxLength(IdentityUserClaimConsts.MaxClaimValueLength);

                b.HasIndex(uc => uc.UserId);
                b.ApplyObjectExtensionMappings();
            });

            builder.Entity<IdentityUserRole>(b =>
            {
                b.ToStarshineTable(nameof(IdentityUserRole))
                    .ConfigureStarshineByConvention();

                b.HasKey(ur => new { ur.UserId, ur.RoleId });
                b.HasIndex(ur => new { ur.RoleId, ur.UserId });
                b.ApplyObjectExtensionMappings();
            });

            builder.Entity<IdentityUserLogin>(b =>
            {
                b.ToStarshineTable(nameof(IdentityUserLogin))
                    .ConfigureStarshineByConvention();

                b.HasKey(x => new { x.UserId, x.LoginProvider });
                b.Property(ul => ul.LoginProvider).HasMaxLength(IdentityUserLoginConsts.MaxLoginProviderLength)
                    .IsRequired();
                b.Property(ul => ul.ProviderKey).HasMaxLength(IdentityUserLoginConsts.MaxProviderKeyLength)
                    .IsRequired();
                b.Property(ul => ul.ProviderDisplayName)
                    .HasMaxLength(IdentityUserLoginConsts.MaxProviderDisplayNameLength);

                b.HasIndex(l => new { l.LoginProvider, l.ProviderKey });
                b.ApplyObjectExtensionMappings();
            });

            builder.Entity<IdentityUserToken>(b =>
            {
                b.ToStarshineTable(nameof(IdentityUserToken))
                    .ConfigureStarshineByConvention();

                b.HasKey(l => new { l.UserId, l.LoginProvider, l.Name });
                b.Property(ul => ul.LoginProvider).HasMaxLength(IdentityUserTokenConsts.MaxLoginProviderLength)
                    .IsRequired();
                b.Property(ul => ul.Name).HasMaxLength(IdentityUserTokenConsts.MaxNameLength).IsRequired();

                b.ApplyObjectExtensionMappings();
            });

            builder.Entity<IdentityRole>(b =>
            {
                b.ToStarshineTable(nameof(IdentityRole))
                    .ConfigureStarshineByConvention();

                b.Property(r => r.Name).IsRequired().HasMaxLength(IdentityRoleConsts.MaxNameLength);
                b.Property(r => r.NormalizedName).IsRequired().HasMaxLength(IdentityRoleConsts.MaxNormalizedNameLength);
                b.Property(r => r.IsDefault);
                b.Property(r => r.IsStatic);
                b.Property(r => r.IsPublic);
                b.Property(r => r.TenantId);

                b.HasIndex(r => r.NormalizedName);
                b.ApplyObjectExtensionMappings();
            });

            builder.Entity<IdentityRoleClaim>(b =>
            {
                b.ToStarshineTable(nameof(IdentityRoleClaim))
                    .ConfigureStarshineByConvention();

                b.Property(x => x.Id).ValueGeneratedNever();
                b.Property(uc => uc.ClaimType).HasMaxLength(IdentityRoleClaimConsts.MaxClaimTypeLength).IsRequired();
                b.Property(uc => uc.ClaimValue).HasMaxLength(IdentityRoleClaimConsts.MaxClaimValueLength);

                b.HasIndex(uc => uc.RoleId);
                b.ApplyObjectExtensionMappings();
            });

            if (builder.IsHostDatabase())
            {
                builder.Entity<IdentityClaimType>(b =>
                {
                    b.ToStarshineTable(nameof(IdentityClaimType))
                        .ConfigureStarshineByConvention();

                    b.Property(uc => uc.Name).HasMaxLength(IdentityClaimTypeConsts.MaxNameLength)
                        .IsRequired(); // make unique
                    b.Property(uc => uc.Regex).HasMaxLength(IdentityClaimTypeConsts.MaxRegexLength);
                    b.Property(uc => uc.RegexDescription).HasMaxLength(IdentityClaimTypeConsts.MaxRegexDescriptionLength);
                    b.Property(uc => uc.Description).HasMaxLength(IdentityClaimTypeConsts.MaxDescriptionLength);

                    b.ApplyObjectExtensionMappings();
                });
            }

            builder.Entity<OrganizationUnit>(b =>
            {
                b.ToStarshineTable(nameof(OrganizationUnit))
                    .ConfigureStarshineByConvention();

                b.Property(x => x.Code).IsRequired().HasMaxLength(OrganizationUnitConsts.MaxCodeLength);
                b.Property(x => x.DisplayName).IsRequired().HasMaxLength(OrganizationUnitConsts.MaxDisplayNameLength);

                b.HasIndex(x => x.Code);
                b.ApplyObjectExtensionMappings();
            });

            builder.Entity<OrganizationUnitRole>(b =>
            {
                b.ToStarshineTable(nameof(OrganizationUnitRole))
                    .ConfigureStarshineByConvention();
                b.HasKey(x => new { x.OrganizationUnitId, x.RoleId });
                b.HasIndex(x => new { x.RoleId, x.OrganizationUnitId });
                b.ApplyObjectExtensionMappings();
            });

            builder.Entity<IdentityUserOrganizationUnit>(b =>
            {
                b.ToStarshineTable(nameof(IdentityUserOrganizationUnit))
                    .ConfigureStarshineByConvention();

                b.HasKey(x => new { x.OrganizationUnitId, x.UserId });
                b.HasIndex(x => new { x.UserId, x.OrganizationUnitId });
                b.ApplyObjectExtensionMappings();
            });

            builder.Entity<IdentitySecurityLog>(b =>
            {
                b.ToStarshineTable(nameof(IdentitySecurityLog))
                    .ConfigureStarshineByConvention();

                b.Property(x => x.TenantName).HasMaxLength(IdentitySecurityLogConsts.MaxTenantNameLength);
                b.Property(x => x.ApplicationName).HasMaxLength(IdentitySecurityLogConsts.MaxApplicationNameLength);
                b.Property(x => x.Identity).HasMaxLength(IdentitySecurityLogConsts.MaxIdentityLength);
                b.Property(x => x.Action).HasMaxLength(IdentitySecurityLogConsts.MaxActionLength);
                b.Property(x => x.UserName).HasMaxLength(IdentitySecurityLogConsts.MaxUserNameLength);
                b.Property(x => x.ClientIpAddress).HasMaxLength(IdentitySecurityLogConsts.MaxClientIpAddressLength);
                b.Property(x => x.ClientId).HasMaxLength(IdentitySecurityLogConsts.MaxClientIdLength);
                b.Property(x => x.CorrelationId).HasMaxLength(IdentitySecurityLogConsts.MaxCorrelationIdLength);
                b.Property(x => x.BrowserInfo).HasMaxLength(IdentitySecurityLogConsts.MaxBrowserInfoLength);
                b.HasIndex(x => new { x.TenantId, x.ApplicationName });
                b.HasIndex(x => new { x.TenantId, x.Identity });
                b.HasIndex(x => new { x.TenantId, x.Action });
                b.HasIndex(x => new { x.TenantId, x.UserId });

                b.ApplyObjectExtensionMappings();
            });

            if (builder.IsHostDatabase())
            {
                builder.Entity<IdentityLinkUser>(b =>
                {
                    b.ToStarshineTable(nameof(IdentityLinkUser));
                    b.HasIndex(x => new
                    {
                        UserId = x.SourceUserId,
                        TenantId = x.SourceTenantId,
                        LinkedUserId = x.TargetUserId,
                        LinkedTenantId = x.TargetTenantId
                    }).IsUnique();

                    b.ApplyObjectExtensionMappings();
                });
            }

            builder.Entity<IdentityUserDelegation>(b =>
            {
                b.ToStarshineTable(nameof(IdentityUserDelegation))
                    .ConfigureStarshineByConvention();

                b.ApplyObjectExtensionMappings();
            });

            builder.Entity<IdentitySession>(b =>
            {
                b.ToStarshineTable(nameof(IdentitySession))
                    .ConfigureStarshineByConvention();

                b.Property(x => x.SessionId).HasMaxLength(IdentitySessionConsts.MaxSessionIdLength).IsRequired();
                b.Property(x => x.Device).HasMaxLength(IdentitySessionConsts.MaxDeviceLength).IsRequired();
                b.Property(x => x.DeviceInfo).HasMaxLength(IdentitySessionConsts.MaxDeviceInfoLength);
                b.Property(x => x.ClientId).HasMaxLength(IdentitySessionConsts.MaxClientIdLength);
                b.Property(x => x.IpAddresses).HasMaxLength(IdentitySessionConsts.MaxIpAddressesLength);
                b.HasIndex(x => x.SessionId);
                b.HasIndex(x => x.Device);
                b.HasIndex(x => new { x.TenantId, x.UserId });
                b.ApplyObjectExtensionMappings();
            });

            builder.TryConfigureObjectExtensions<Volo.Abp.Identity.EntityFrameworkCore.IdentityDbContext>();
        }
    }
}
