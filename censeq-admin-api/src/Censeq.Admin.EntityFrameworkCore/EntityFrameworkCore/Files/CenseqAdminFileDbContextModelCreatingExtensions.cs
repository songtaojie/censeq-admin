using Censeq.Admin.Files;
using Censeq.Framework.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;

namespace Censeq.Admin.EntityFrameworkCore;

/// <summary>
/// 文件管理模块的 EF Core 模型配置扩展。
/// </summary>
public static class CenseqAdminFileDbContextModelCreatingExtensions
{
    /// <summary>
    /// 配置文件记录表、字段长度和查询索引。
    /// </summary>
    public static void ConfigureAdminFiles(this ModelBuilder builder)
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
            b.HasIndex(x => new { x.TenantId, x.Category, x.CreationTime });
            b.HasIndex(x => x.Url);
            b.ApplyObjectExtensionMappings();
        });
    }
}
