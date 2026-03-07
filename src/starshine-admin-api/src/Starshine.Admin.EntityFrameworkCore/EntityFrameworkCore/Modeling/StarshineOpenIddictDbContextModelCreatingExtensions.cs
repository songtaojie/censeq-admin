using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.OpenIddict.Applications;
using Volo.Abp.OpenIddict.Authorizations;
using Volo.Abp.OpenIddict.Scopes;
using Volo.Abp.OpenIddict.Tokens;
using Volo.Abp;

namespace Starshine.Admin.EntityFrameworkCore.Modeling
{
    internal static class StarshineOpenIddictDbContextModelCreatingExtensions
    {
        public static void ConfigureOpenIddict(this ModelBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            if (builder.IsTenantOnlyDatabase())
            {
                return;
            }

            builder.Entity<OpenIddictApplication>(b =>
            {
                b.ToStarshineTable(nameof(OpenIddictApplication))
                    .ConfigureStarshineByConvention();

                b.HasIndex(x => x.ClientId).IsUnique();

                b.Property(x => x.ApplicationType).HasMaxLength(OpenIddictApplicationConsts.ApplicationTypeMaxLength);
                b.Property(x => x.ClientId).HasMaxLength(OpenIddictApplicationConsts.ClientIdMaxLength);
                b.Property(x => x.ConsentType).HasMaxLength(OpenIddictApplicationConsts.ConsentTypeMaxLength);
                b.Property(x => x.ClientType).HasMaxLength(OpenIddictApplicationConsts.ClientTypeMaxLength);

                b.ApplyObjectExtensionMappings();
            });

            builder.Entity<OpenIddictAuthorization>(b =>
            {
                b.ToStarshineTable(nameof(OpenIddictAuthorization))
                    .ConfigureStarshineByConvention();

                b.HasIndex(x => new
                {
                    x.ApplicationId,
                    x.Status,
                    x.Subject,
                    x.Type
                });

                b.Property(x => x.Status).HasMaxLength(OpenIddictAuthorizationConsts.StatusMaxLength);
                b.Property(x => x.Subject).HasMaxLength(OpenIddictAuthorizationConsts.SubjectMaxLength);
                b.Property(x => x.Type).HasMaxLength(OpenIddictAuthorizationConsts.TypeMaxLength);

                b.ApplyObjectExtensionMappings();
            });

            builder.Entity<OpenIddictScope>(b =>
            {
                b.ToStarshineTable(nameof(OpenIddictScope))
                    .ConfigureStarshineByConvention();

                b.HasIndex(x => x.Name).IsUnique();
                b.Property(x => x.Name).HasMaxLength(OpenIddictScopeConsts.NameMaxLength);
                b.ApplyObjectExtensionMappings();
            });

            builder.Entity<OpenIddictToken>(b =>
            {
                b.ToStarshineTable(nameof(OpenIddictToken))
                    .ConfigureStarshineByConvention();

                b.HasIndex(x => x.ReferenceId).IsUnique();

                b.HasIndex(x => new
                {
                    x.ApplicationId,
                    x.Status,
                    x.Subject,
                    x.Type
                });
                b.Property(x => x.ReferenceId).HasMaxLength(OpenIddictTokenConsts.ReferenceIdMaxLength);
                b.Property(x => x.Status).HasMaxLength(OpenIddictTokenConsts.StatusMaxLength);
                b.Property(x => x.Subject).HasMaxLength(OpenIddictTokenConsts.SubjectMaxLength);
                b.Property(x => x.Type).HasMaxLength(OpenIddictTokenConsts.TypeMaxLength);

                b.ApplyObjectExtensionMappings();
            });

        }
    }
}
