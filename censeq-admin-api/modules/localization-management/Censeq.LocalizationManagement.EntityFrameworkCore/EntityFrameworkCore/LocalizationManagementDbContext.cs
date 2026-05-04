using Censeq.LocalizationManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Censeq.LocalizationManagement.EntityFrameworkCore;

[ConnectionStringName(CenseqLocalizationManagementDbProperties.ConnectionStringName)]
public class LocalizationManagementDbContext
    : AbpDbContext<LocalizationManagementDbContext>, ILocalizationManagementDbContext
{
    public DbSet<LocalizationResource> LocalizationResources { get; set; } = null!;

    public DbSet<LocalizationCulture> LocalizationCultures { get; set; } = null!;

    public DbSet<LocalizationText> LocalizationTexts { get; set; } = null!;

    public LocalizationManagementDbContext(DbContextOptions<LocalizationManagementDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureLocalizationManagement();
    }
}
