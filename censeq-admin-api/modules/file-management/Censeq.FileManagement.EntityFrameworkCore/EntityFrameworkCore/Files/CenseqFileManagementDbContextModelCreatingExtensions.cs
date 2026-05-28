using Censeq.FileManagement.Files;
using Censeq.Framework.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;

namespace Censeq.FileManagement.EntityFrameworkCore;

public static class CenseqFileManagementDbContextModelCreatingExtensions
{
    public static void ConfigureFileManagement(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        builder.Entity<FileRecord>(b =>
        {
            b.ToCenseqTable(nameof(FileRecord)).ConfigureCenseqByConvention();
            b.Property(x => x.OriginalName).IsRequired().HasMaxLength(FileRecordConsts.MaxNameLength);
            b.Property(x => x.FileName).IsRequired().HasMaxLength(FileRecordConsts.MaxNameLength);
            b.Property(x => x.Extension).IsRequired().HasMaxLength(32);
            b.Property(x => x.ContentType).IsRequired().HasMaxLength(FileRecordConsts.MaxContentTypeLength);
            b.Property(x => x.RelativePath).IsRequired().HasMaxLength(FileRecordConsts.MaxPathLength);
            b.Property(x => x.Url).IsRequired().HasMaxLength(FileRecordConsts.MaxPathLength);
            b.Property(x => x.Hash).HasMaxLength(FileRecordConsts.MaxHashLength);
            b.Property(x => x.Category).HasMaxLength(FileRecordConsts.MaxCategoryLength);
            b.Property(x => x.Provider).IsRequired().HasMaxLength(FileRecordConsts.MaxProviderLength);
            b.Property(x => x.BucketName).HasMaxLength(FileRecordConsts.MaxBucketNameLength);
            b.HasIndex(x => new { x.TenantId, x.Category, x.CreationTime });
            b.HasIndex(x => x.Url);
            b.ApplyObjectExtensionMappings();
        });

        builder.Entity<FileProvider>(b =>
        {
            b.ToCenseqTable(nameof(FileProvider)).ConfigureCenseqByConvention();
            b.Property(x => x.Provider).IsRequired().HasMaxLength(FileProviderConsts.MaxProviderLength);
            b.Property(x => x.BucketName).IsRequired().HasMaxLength(FileProviderConsts.MaxBucketNameLength);
            b.Property(x => x.AccessKey).HasMaxLength(FileProviderConsts.MaxAccessKeyLength);
            b.Property(x => x.SecretKey).HasMaxLength(FileProviderConsts.MaxSecretKeyLength);
            b.Property(x => x.Region).HasMaxLength(FileProviderConsts.MaxRegionLength);
            b.Property(x => x.Endpoint).HasMaxLength(FileProviderConsts.MaxEndpointLength);
            b.Property(x => x.CustomDomain).HasMaxLength(FileProviderConsts.MaxCustomDomainLength);
            b.Property(x => x.Remark).HasMaxLength(FileProviderConsts.MaxRemarkLength);
            b.HasIndex(x => new { x.TenantId, x.IsEnable, x.IsDefault, x.OrderNo });
            b.HasIndex(x => new { x.Provider, x.BucketName });
            b.ApplyObjectExtensionMappings();
        });
    }
}
