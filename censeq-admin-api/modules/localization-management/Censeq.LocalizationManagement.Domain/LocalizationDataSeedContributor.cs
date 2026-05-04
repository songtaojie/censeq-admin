using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Censeq.LocalizationManagement.Entities;
using LocalizationResourceEntity = Censeq.LocalizationManagement.Entities.LocalizationResource;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Localization;

namespace Censeq.LocalizationManagement;

/// <summary>
/// 将静态 JSON 翻译文件的内容同步到数据库（Host 级别，仅插入不覆盖）。
/// 已存在于数据库的条目会跳过，以保留用户的自定义修改。
/// </summary>
public class LocalizationDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly ILocalizationTextRepository _textRepository;
    private readonly ILocalizationResourceRepository _resourceRepository;
    private readonly ILocalizationCultureRepository _cultureRepository;
    private readonly IOptions<AbpLocalizationOptions> _localizationOptions;
    private readonly IGuidGenerator _guidGenerator;
    private readonly IServiceProvider _serviceProvider;

    public LocalizationDataSeedContributor(
        ILocalizationTextRepository textRepository,
        ILocalizationResourceRepository resourceRepository,
        ILocalizationCultureRepository cultureRepository,
        IOptions<AbpLocalizationOptions> localizationOptions,
        IGuidGenerator guidGenerator,
        IServiceProvider serviceProvider)
    {
        _textRepository = textRepository;
        _resourceRepository = resourceRepository;
        _cultureRepository = cultureRepository;
        _localizationOptions = localizationOptions;
        _guidGenerator = guidGenerator;
        _serviceProvider = serviceProvider;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        // 只对 Host 执行种子（租户的翻译由租户管理员自行维护）
        if (context.TenantId != null) return;

        var options = _localizationOptions.Value;

        // ── 1. 同步 LocalizationCulture（Host 级，TenantId = null）──────────
        foreach (var language in options.Languages)
        {
            var existing = await _cultureRepository.FindAsync(language.CultureName, tenantId: null);
            if (existing == null)
            {
                await _cultureRepository.InsertAsync(
                    new LocalizationCulture(
                        _guidGenerator.Create(),
                        cultureName: language.CultureName,
                        displayName: language.DisplayName,
                        isEnabled: true,
                        uiCultureName: language.UiCultureName,
                        tenantId: null
                    ),
                    autoSave: false
                );
            }
        }

        // ── 2. 同步 LocalizationResource + LocalizationText ─────────────────
        foreach (var resource in options.Resources.Values)
        {
            // 只读取静态（非 DB）贡献者，即 JSON 文件贡献者
            var staticContributors = resource.Contributors
                .Where(c => !c.IsDynamic)
                .ToList();

            if (!staticContributors.Any()) continue;

            // 确保 LocalizationResource 记录存在
            var existingResource = await _resourceRepository.FindByNameAsync(resource.ResourceName);
            if (existingResource == null)
            {
                await _resourceRepository.InsertAsync(
                    new LocalizationResourceEntity(
                        _guidGenerator.Create(),
                        name: resource.ResourceName,
                        defaultCultureName: resource.DefaultCultureName
                    ),
                    autoSave: false
                );
            }

            // 必须先 Initialize，否则虚拟文件贡献者无法解析 IVirtualFileProvider
            var initContext = new LocalizationResourceInitializationContext(resource, _serviceProvider);
            foreach (var contributor in staticContributors)
            {
                contributor.Initialize(initContext);
            }

            foreach (var language in options.Languages)
            {
                var cultureName = language.CultureName;
                var dictionary = new Dictionary<string, LocalizedString>();

                // 逐个 JSON 贡献者填充字典（后者覆盖前者，与运行时行为一致）
                foreach (var contributor in staticContributors)
                {
                    await contributor.FillAsync(cultureName, dictionary);
                }

                if (!dictionary.Any()) continue;

                foreach (var (key, localizedString) in dictionary)
                {
                    // 已存在则跳过，保留用户在数据库中的自定义修改
                    var existingText = await _textRepository.FindAsync(
                        resource.ResourceName, cultureName, key, tenantId: null);

                    if (existingText != null) continue;

                    await _textRepository.InsertAsync(
                        new LocalizationText(
                            _guidGenerator.Create(),
                            resource.ResourceName,
                            cultureName,
                            key,
                            localizedString.Value
                        ),
                        autoSave: false
                    );
                }
            }
        }
    }
}
