using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.Features;

namespace Censeq.FeatureManagement;

[Authorize]
public class FeatureAppService : FeatureManagementAppServiceBase, IFeatureAppService
{
    protected FeatureManagementOptions Options { get; }
    protected IFeatureManager FeatureManager { get; }
    protected IFeatureDefinitionManager FeatureDefinitionManager { get; }

    public FeatureAppService(IFeatureManager featureManager,
        IFeatureDefinitionManager featureDefinitionManager,
        IOptions<FeatureManagementOptions> options)
    {
        FeatureManager = featureManager;
        FeatureDefinitionManager = featureDefinitionManager;
        Options = options.Value;
    }

    /// <summary>
    /// 将查询串中的空 / 空白 providerKey 视为未指定，与 ABP 宿主侧「租户默认特性」语义一致，并避免非空 string 触发的隐式 Required 校验。
    /// </summary>
    protected static string? NormalizeFeatureProviderKey(string? providerKey)
    {
        return string.IsNullOrWhiteSpace(providerKey) ? null : providerKey;
    }

    protected static string ProviderKeyForStore(string? providerKey)
    {
        return NormalizeFeatureProviderKey(providerKey) ?? string.Empty;
    }

    public virtual async Task<GetFeatureListResultDto> GetAsync([NotNull] string providerName, string? providerKey)
    {
        var pk = NormalizeFeatureProviderKey(providerKey);
        await CheckProviderPolicy(providerName, pk);

        var result = new GetFeatureListResultDto
        {
            Groups = new List<FeatureGroupDto>()
        };

        foreach (var group in await FeatureDefinitionManager.GetGroupsAsync())
        {
            var groupDto = CreateFeatureGroupDto(group);

            foreach (var featureDefinition in group.GetFeaturesWithChildren())
            {
                if (providerName == TenantFeatureValueProvider.ProviderName &&
                    CurrentTenant.Id == null &&
                    pk == null &&
                    !featureDefinition.IsAvailableToHost)
                {
                    continue;
                }

                var feature = await FeatureManager.GetOrNullWithProviderAsync(
                    featureDefinition.Name,
                    providerName,
                    ProviderKeyForStore(providerKey));
                groupDto.Features.Add(CreateFeatureDto(feature, featureDefinition));
            }

            SetFeatureDepth(groupDto.Features, providerName, pk);

            if (groupDto.Features.Any())
            {
                result.Groups.Add(groupDto);
            }
        }

        return result;
    }

    private FeatureGroupDto CreateFeatureGroupDto(FeatureGroupDefinition groupDefinition)
    {
        return new FeatureGroupDto
        {
            Name = groupDefinition.Name,
            DisplayName = groupDefinition.DisplayName != null
                ? (string)groupDefinition.DisplayName.Localize(StringLocalizerFactory)! ?? string.Empty
                : string.Empty,
            Features = new List<FeatureDto>()
        };
    }

    private FeatureDto CreateFeatureDto(FeatureNameValueWithGrantedProvider featureNameValueWithGrantedProvider, FeatureDefinition featureDefinition)
    {
        return new FeatureDto
        {
            Name = featureDefinition.Name,
            DisplayName = featureDefinition.DisplayName != null
                ? (string)featureDefinition.DisplayName.Localize(StringLocalizerFactory)! ?? string.Empty
                : string.Empty,
            Description = featureDefinition.Description != null
                ? (string)featureDefinition.Description.Localize(StringLocalizerFactory)! ?? string.Empty
                : string.Empty,
            DefaultValue = featureDefinition.DefaultValue ?? string.Empty,
            IsVisibleToClients = featureDefinition.IsVisibleToClients,
            IsAvailableToHost = featureDefinition.IsAvailableToHost,
            AllowedProviders = featureDefinition.AllowedProviders?.ToList() ?? new List<string>(),

            ValueType = featureDefinition.ValueType!,

            ParentName = featureDefinition.Parent?.Name ?? string.Empty,
            Value = featureNameValueWithGrantedProvider.Value ?? string.Empty,
            Provider = new FeatureProviderDto
            {
                Name = featureNameValueWithGrantedProvider.Provider?.Name ?? string.Empty,
                Key = featureNameValueWithGrantedProvider.Provider?.Key ?? string.Empty
            }
        };
    }

    public virtual async Task UpdateAsync([NotNull] string providerName, string? providerKey, UpdateFeaturesDto input)
    {
        var pk = NormalizeFeatureProviderKey(providerKey);
        await CheckProviderPolicy(providerName, pk);

        foreach (var feature in input.Features)
        {
            await FeatureManager.SetAsync(feature.Name, feature.Value, providerName, ProviderKeyForStore(providerKey));
        }
    }

    protected virtual void SetFeatureDepth(List<FeatureDto> features, string providerName, string? providerKey,
        FeatureDto? parentFeature = null, int depth = 0)
    {
        foreach (var feature in features)
        {
            if ((parentFeature == null && feature.ParentName == null) || (parentFeature != null && parentFeature.Name == feature.ParentName))
            {
                feature.Depth = depth;
                SetFeatureDepth(features, providerName, providerKey, feature, depth + 1);
            }
        }
    }

    protected virtual async Task CheckProviderPolicy(string providerName, string? providerKey)
    {
        string policyName;
        if (providerName == TenantFeatureValueProvider.ProviderName && CurrentTenant.Id == null && providerKey == null)
        {
            policyName = FeatureManagementPermissions.ManageHostFeatures;
        }
        else
        {
            policyName = Options.ProviderPolicies.GetOrDefault(providerName) ?? string.Empty;
            if (policyName.IsNullOrEmpty())
            {
                throw new AbpException($"No policy defined to get/set permissions for the provider '{providerName}'. Use {nameof(FeatureManagementOptions)} to map the policy.");
            }
        }

        await AuthorizationService.CheckAsync(policyName);
    }

    public virtual async Task DeleteAsync([NotNull] string providerName, string? providerKey)
    {
        var pk = NormalizeFeatureProviderKey(providerKey);
        await CheckProviderPolicy(providerName, pk);
        await FeatureManager.DeleteAsync(providerName, ProviderKeyForStore(providerKey));
    }
}
