using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Censeq.LocalizationManagement.Entities;
using Volo.Abp.Domain.Repositories;

namespace Censeq.LocalizationManagement;

public interface ILocalizationResourceRepository : IRepository<LocalizationResource, Guid>
{
    Task<LocalizationResource?> FindByNameAsync(string name, CancellationToken cancellationToken = default);

    Task<List<LocalizationResource>> GetAllAsync(CancellationToken cancellationToken = default);
}
