using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Starshine.Admin.EntityFrameworkCore.Modeling
{
    internal static class StarshineBackgroundJobsDbContextModelCreatingExtensions
    {
        public static void ConfigureBackgroundJobs(this ModelBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            if (builder.IsTenantOnlyDatabase())
            {
                return;
            }

            builder.Entity<BackgroundJobRecord>(b =>
            {
                b.ToStarshineTable(nameof(BackgroundJobRecord))
                    .ConfigureStarshineByConvention();

                b.Property(x => x.JobName).IsRequired().HasMaxLength(BackgroundJobRecordConsts.MaxJobNameLength);
                b.Property(x => x.JobArgs).IsRequired().HasMaxLength(BackgroundJobRecordConsts.MaxJobArgsLength);
                b.Property(x => x.TryCount).HasDefaultValue(0);
                b.Property(x => x.NextTryTime);
                b.Property(x => x.LastTryTime);
                b.Property(x => x.IsAbandoned).HasDefaultValue(false);
                b.Property(x => x.Priority).HasDefaultValue(BackgroundJobPriority.Normal).HasSentinel(BackgroundJobPriority.Normal);
                b.HasIndex(x => new { x.IsAbandoned, x.NextTryTime });
                b.ApplyObjectExtensionMappings();
            });

            builder.TryConfigureObjectExtensions<BackgroundJobsDbContext>();
        }
    }
}
