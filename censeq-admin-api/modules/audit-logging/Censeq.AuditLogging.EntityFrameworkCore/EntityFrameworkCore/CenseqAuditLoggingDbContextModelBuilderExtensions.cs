using Censeq.AuditLogging.Entities;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Censeq.AuditLogging.EntityFrameworkCore
{
    internal static class StarshineAuditLoggingDbContextModelBuilderExtensions
    {
        internal static void ConfigureAuditLogging(this ModelBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            builder.Entity<AuditLog>(b =>
            {
                b.ToCenseqTable(nameof(AuditLog))
                    .ConfigureStarshineByConvention();

                b.Property(x => x.ApplicationName).HasMaxLength(AuditLogConsts.MaxApplicationNameLength);
                b.Property(x => x.ClientIpAddress).HasMaxLength(AuditLogConsts.MaxClientIpAddressLength);
                b.Property(x => x.ClientName).HasMaxLength(AuditLogConsts.MaxClientNameLength);
                b.Property(x => x.ClientId).HasMaxLength(AuditLogConsts.MaxClientIdLength);
                b.Property(x => x.CorrelationId).HasMaxLength(AuditLogConsts.MaxCorrelationIdLength);
                b.Property(x => x.BrowserInfo).HasMaxLength(AuditLogConsts.MaxBrowserInfoLength);
                b.Property(x => x.HttpMethod).HasMaxLength(AuditLogConsts.MaxHttpMethodLength);
                b.Property(x => x.Url).HasMaxLength(AuditLogConsts.MaxUrlLength);
                b.Property(x => x.HttpStatusCode);

                b.Property(x => x.Comments).HasMaxLength(AuditLogConsts.MaxCommentsLength);
                b.Property(x => x.ExecutionDuration);
                b.Property(x => x.ImpersonatorTenantId);
                b.Property(x => x.ImpersonatorUserId);
                b.Property(x => x.ImpersonatorTenantName).HasMaxLength(AuditLogConsts.MaxTenantNameLength);
                b.Property(x => x.ImpersonatorUserName).HasMaxLength(AuditLogConsts.MaxUserNameLength);
                b.Property(x => x.UserId);
                b.Property(x => x.UserName).HasMaxLength(AuditLogConsts.MaxUserNameLength);
                b.Property(x => x.TenantId);
                b.Property(x => x.TenantName).HasMaxLength(AuditLogConsts.MaxTenantNameLength);

                b.HasIndex(x => new { x.TenantId, x.ExecutionTime });
                b.HasIndex(x => new { x.TenantId, x.UserId, x.ExecutionTime });

                b.ApplyObjectExtensionMappings();
            });

            builder.Entity<AuditLogAction>(b =>
            {
                b.ToCenseqTable(nameof(AuditLogAction))
                    .ConfigureStarshineByConvention();

                b.Property(x => x.AuditLogId);
                b.Property(x => x.ServiceName).HasMaxLength(AuditLogActionConsts.MaxServiceNameLength);
                b.Property(x => x.MethodName).HasMaxLength(AuditLogActionConsts.MaxMethodNameLength);
                b.Property(x => x.Parameters).HasMaxLength(AuditLogActionConsts.MaxParametersLength);
                b.Property(x => x.ExecutionTime);
                b.Property(x => x.ExecutionDuration);

                b.HasIndex(x => new { x.AuditLogId });
                b.HasIndex(x => new { x.TenantId, x.ServiceName, x.MethodName, x.ExecutionTime });

                b.ApplyObjectExtensionMappings();
            });

            builder.Entity<EntityChange>(b =>
            {
                b.ToCenseqTable(nameof(EntityChange))
                    .ConfigureStarshineByConvention();

                b.Property(x => x.EntityTypeFullName).HasMaxLength(EntityChangeConsts.MaxEntityTypeFullNameLength).IsRequired();
                b.Property(x => x.EntityId).HasMaxLength(EntityChangeConsts.MaxEntityIdLength);
                b.Property(x => x.AuditLogId).IsRequired();
                b.Property(x => x.ChangeTime).IsRequired();
                b.Property(x => x.ChangeType).IsRequired();
                b.Property(x => x.TenantId);

                b.HasIndex(x => new { x.AuditLogId });
                b.HasIndex(x => new { x.TenantId, x.EntityTypeFullName, x.EntityId });
                b.ApplyObjectExtensionMappings();
            });

            builder.Entity<EntityPropertyChange>(b =>
            {
                b.ToCenseqTable(nameof(EntityPropertyChange))
                    .ConfigureStarshineByConvention();

                b.Property(x => x.NewValue).HasMaxLength(EntityPropertyChangeConsts.MaxNewValueLength);
                b.Property(x => x.PropertyName).HasMaxLength(EntityPropertyChangeConsts.MaxPropertyNameLength).IsRequired();
                b.Property(x => x.PropertyTypeFullName).HasMaxLength(EntityPropertyChangeConsts.MaxPropertyTypeFullNameLength).IsRequired();
                b.Property(x => x.OriginalValue).HasMaxLength(EntityPropertyChangeConsts.MaxOriginalValueLength);
                b.HasIndex(x => new { x.EntityChangeId });
                b.ApplyObjectExtensionMappings();
            });
        }
    }
}
