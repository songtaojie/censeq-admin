using Censeq.SettingManagement.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Settings;

namespace Censeq.SettingManagement;

public interface ISettingDefinitionSerializer
{
    Task<SettingDefinitionRecord> SerializeAsync(SettingDefinition setting);

    Task<List<SettingDefinitionRecord>> SerializeAsync(IEnumerable<SettingDefinition> settings);
}
