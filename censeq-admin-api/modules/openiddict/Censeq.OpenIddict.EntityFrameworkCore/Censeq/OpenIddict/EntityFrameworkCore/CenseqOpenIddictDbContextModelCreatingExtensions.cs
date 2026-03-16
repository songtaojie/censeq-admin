using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Censeq.OpenIddict.Applications;
using Censeq.OpenIddict.Authorizations;
using Censeq.OpenIddict.Scopes;
using Censeq.OpenIddict.Tokens;

namespace Censeq.OpenIddict.EntityFrameworkCore;

public static class CenseqOpenIddictDbContextModelCreatingExtensions
{
    public static void ConfigureOpenIddict(
        this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        if (builder.IsTenantOnlyDatabase())
        {
            return;
        }

        builder.Entity<OpenIddictApplication>(b =>
        {
            b.ToTable(CenseqOpenIddictDbProperties.DbTablePrefix + "Applications", CenseqOpenIddictDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.HasIndex(x => x.ClientId);
                //.IsUnique();

            b.Property(x => x.ApplicationType)
                .HasMaxLength(OpenIddictApplicationConsts.ApplicationTypeMaxLength);

            b.Property(x => x.ClientId)
                .HasMaxLength(OpenIddictApplicationConsts.ClientIdMaxLength);

            b.Property(x => x.ConsentType)
                .HasMaxLength(OpenIddictApplicationConsts.ConsentTypeMaxLength);

            b.Property(x => x.ClientType)
                .HasMaxLength(OpenIddictApplicationConsts.ClientTypeMaxLength);

            b.ApplyObjectExtensionMappings();
        });

        builder.Entity<OpenIddictAuthorization>(b =>
        {
            b.ToTable(CenseqOpenIddictDbProperties.DbTablePrefix + "Authorizations", CenseqOpenIddictDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.HasIndex(x => new
            {
                x.ApplicationId,
                x.Status,
                x.Subject,
                x.Type
            });

            b.Property(x => x.Status)
                .HasMaxLength(OpenIddictAuthorizationConsts.StatusMaxLength);

            b.Property(x => x.Subject)
                .HasMaxLength(OpenIddictAuthorizationConsts.SubjectMaxLength);

            b.Property(x => x.Type)
                .HasMaxLength(OpenIddictAuthorizationConsts.TypeMaxLength);

            b.HasOne<OpenIddictApplication>().WithMany().HasForeignKey(x => x.ApplicationId).IsRequired(false);

            b.ApplyObjectExtensionMappings();
        });

        builder.Entity<OpenIddictScope>(b =>
        {
            b.ToTable(CenseqOpenIddictDbProperties.DbTablePrefix + "Scopes", CenseqOpenIddictDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.HasIndex(x => x.Name);
                //.IsUnique();

            b.Property(x => x.Name)
                .HasMaxLength(OpenIddictScopeConsts.NameMaxLength);

            b.ApplyObjectExtensionMappings();
        });

        builder.Entity<OpenIddictToken>(b =>
        {
            b.ToTable(CenseqOpenIddictDbProperties.DbTablePrefix + "Tokens", CenseqOpenIddictDbProperties.DbSchema);

            b.ConfigureByConvention();

            b.HasIndex(x => x.ReferenceId);
                //.IsUnique();

            b.HasIndex(x => new
            {
                x.ApplicationId,
                x.Status,
                x.Subject,
                x.Type
            });

            b.Property(x => x.ReferenceId)
                .HasMaxLength(OpenIddictTokenConsts.ReferenceIdMaxLength);

            b.Property(x => x.Status)
                .HasMaxLength(OpenIddictTokenConsts.StatusMaxLength);

            b.Property(x => x.Subject)
                .HasMaxLength(OpenIddictTokenConsts.SubjectMaxLength);

            b.Property(x => x.Type)
                .HasMaxLength(OpenIddictTokenConsts.TypeMaxLength);

            b.HasOne<OpenIddictApplication>().WithMany().HasForeignKey(x => x.ApplicationId).IsRequired(false);
            b.HasOne<OpenIddictAuthorization>().WithMany().HasForeignKey(x => x.AuthorizationId).IsRequired(false);

            b.ApplyObjectExtensionMappings();
        });

    }
}
