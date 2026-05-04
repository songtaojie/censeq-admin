using Censeq.LocalizationManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Censeq.LocalizationManagement.EntityFrameworkCore;

[ConnectionStringName(CenseqLocalizationManagementDbProperties.ConnectionStringName)]
public interface ILocalizationManagementDbContext : IEfCoreDbContext
{
    DbSet<LocalizationResource> LocalizationResources { get; }

    DbSet<LocalizationCulture> LocalizationCultures { get; }

    DbSet<LocalizationText> LocalizationTexts { get; }
}
