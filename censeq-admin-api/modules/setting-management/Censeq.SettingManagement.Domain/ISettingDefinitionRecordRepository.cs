using Censeq.SettingManagement.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Censeq.SettingManagement;

public interface ISettingDefinitionRecordRepository : IBasicRepository<SettingDefinitionRecord, Guid>
{
    Task<SettingDefinitionRecord?> FindByNameAsync(string name, CancellationToken cancellationToken = default);

    Task<List<SettingDefinitionRecord>> GetPagedListAsync(string? filter, int skipCount, int maxResultCount, CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(string? filter, CancellationToken cancellationToken = default);
}
