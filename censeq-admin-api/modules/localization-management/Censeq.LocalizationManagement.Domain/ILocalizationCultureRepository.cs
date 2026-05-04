using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Censeq.LocalizationManagement.Entities;
using Volo.Abp.Domain.Repositories;

namespace Censeq.LocalizationManagement;

public interface ILocalizationCultureRepository : IRepository<LocalizationCulture, Guid>
{
    Task<LocalizationCulture?> FindAsync(
        string cultureName,
        Guid? tenantId,
        CancellationToken cancellationToken = default);

    Task<List<LocalizationCulture>> GetListAsync(
        Guid? tenantId,
        bool? isEnabled = null,
        CancellationToken cancellationToken = default);
}
