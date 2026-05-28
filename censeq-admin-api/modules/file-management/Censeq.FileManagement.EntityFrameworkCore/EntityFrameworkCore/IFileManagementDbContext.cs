using Censeq.FileManagement.Files;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Censeq.FileManagement.EntityFrameworkCore;

[ConnectionStringName(ConnectionStrings.DefaultConnectionStringName)]
public interface IFileManagementDbContext : IEfCoreDbContext
{
    DbSet<FileRecord> FileRecords { get; set; }

    DbSet<FileProvider> FileProviders { get; set; }
}
