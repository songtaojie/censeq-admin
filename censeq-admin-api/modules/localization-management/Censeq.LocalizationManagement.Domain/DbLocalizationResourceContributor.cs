using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace Censeq.LocalizationManagement;

/// <summary>
/// 从数据库读取翻译覆盖值的本地化资源贡献者。
/// 优先级：租户级 DB > Host 级 DB > JSON 文件
/// （JSON contributor 先加载，本 contributor 后加载并覆盖）
/// </summary>
public class DbLocalizationResourceContributor : ILocalizationResourceContributor
{
    private string? _resourceName;
    private IServiceProvider? _serviceProvider;

    public bool IsDynamic => true;

    public void Initialize(LocalizationResourceInitializationContext context)
    {
        _serviceProvider = context.ServiceProvider;
        _resourceName = context.Resource.ResourceName;
    }

    public LocalizedString? GetOrNull(string cultureName, string name)
    {
        return GetOrNullAsync(cultureName, name).GetAwaiter().GetResult();
    }

    private async Task<LocalizedString?> GetOrNullAsync(string cultureName, string name)
    {
        if (_serviceProvider == null || _resourceName == null) return null;

        using var scope = _serviceProvider.CreateScope();

        var textRepository = scope.ServiceProvider.GetService<ILocalizationTextRepository>();
        if (textRepository == null) return null;

        var currentTenant = scope.ServiceProvider.GetService<ICurrentTenant>();

        // 优先查租户级翻译
        if (currentTenant?.Id != null)
        {
            var tenantText = await textRepository.FindAsync(_resourceName, cultureName, name, currentTenant.Id);
            if (!string.IsNullOrWhiteSpace(tenantText?.Value))
                return new LocalizedString(name, tenantText!.Value);
        }

        // 回退到 Host 级翻译
        var hostText = await textRepository.FindAsync(_resourceName, cultureName, name, tenantId: null);
        if (!string.IsNullOrWhiteSpace(hostText?.Value))
            return new LocalizedString(name, hostText!.Value);

        return null;
    }

    public void Fill(string cultureName, Dictionary<string, LocalizedString> dictionary)
    {
        FillAsync(cultureName, dictionary).GetAwaiter().GetResult();
    }

    public async Task FillAsync(string cultureName, Dictionary<string, LocalizedString> dictionary)
    {
        if (_serviceProvider == null || _resourceName == null) return;

        using var scope = _serviceProvider.CreateScope();

        var textRepository = scope.ServiceProvider.GetService<ILocalizationTextRepository>();
        if (textRepository == null) return;

        // Step 1: 填充 Host 级翻译（TenantId = null）
        var hostTexts = await textRepository.GetListAsync(_resourceName, cultureName, tenantId: null);
        foreach (var text in hostTexts)
        {
            if (!string.IsNullOrWhiteSpace(text.Value))
            {
                dictionary[text.Key] = new LocalizedString(text.Key, text.Value);
            }
        }

        // Step 2: 若当前有租户上下文，用租户翻译覆盖 Host 翻译
        var currentTenant = scope.ServiceProvider.GetService<ICurrentTenant>();
        if (currentTenant?.Id != null)
        {
            var tenantTexts = await textRepository.GetListAsync(_resourceName, cultureName, currentTenant.Id);
            foreach (var text in tenantTexts)
            {
                if (!string.IsNullOrWhiteSpace(text.Value))
                {
                    dictionary[text.Key] = new LocalizedString(text.Key, text.Value);
                }
            }
        }
    }

    public async Task<IEnumerable<string>> GetSupportedCulturesAsync()
    {
        if (_serviceProvider == null || _resourceName == null)
            return Array.Empty<string>();

        using var scope = _serviceProvider.CreateScope();

        var textRepository = scope.ServiceProvider.GetService<ILocalizationTextRepository>();
        if (textRepository == null) return Array.Empty<string>();

        var currentTenant = scope.ServiceProvider.GetService<ICurrentTenant>();

        // 返回当前租户（或 Host）在该资源下存在翻译的语言列表
        var tenantId = currentTenant?.Id;
        var cultures = await textRepository.GetDistinctCultureNamesAsync(_resourceName, tenantId);

        // 若是租户，还需合并 Host 级的语言（Host 翻译也会作为 fallback）
        if (tenantId != null)
        {
            var hostCultures = await textRepository.GetDistinctCultureNamesAsync(_resourceName, tenantId: null);
            cultures = cultures.Union(hostCultures).Distinct().ToList();
        }

        return cultures;
    }
}
