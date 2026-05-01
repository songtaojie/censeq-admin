using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Censeq.Identity;

/// <summary>
/// I身份Data种子数据接口
/// </summary>
public interface IIdentityDataSeeder
{
    Task<IdentityDataSeedResult> SeedAsync(
        [NotNull] string adminEmail,
        [NotNull] string adminPassword,
        Guid? tenantId = null,
        string? adminUserName = null,
        string? adminName = null);
}
