import type { GetFeatureListResultDto, UpdateFeaturesDto } from '/@/api/models/feature-management';
import { useBaseApi } from '../base';

const api = useBaseApi('feature-management');

/**
 * 特性取值维护：按 Feature 体系中的 Provider（如租户 T）+ ProviderKey（租户 Id）读写。
 * 路由与 FeaturesController 一致。
 */
export function useFeatureManagementApi() {
	const basePath = 'api/feature-management/features';

	return {
		getFeatures: async (providerName: string, providerKey: string | null | undefined): Promise<GetFeatureListResultDto> => {
			return await api.request<GetFeatureListResultDto>(basePath, 'GET', {
				providerName,
				providerKey: providerKey ?? '',
			});
		},

		updateFeatures: async (
			providerName: string,
			providerKey: string | null | undefined,
			input: UpdateFeaturesDto
		): Promise<void> => {
			const qs = new URLSearchParams({ providerName, providerKey: providerKey ?? '' });
			return await api.request<void>(`${basePath}?${qs.toString()}`, 'PUT', input);
		},

		deleteFeatureOverrides: async (providerName: string, providerKey: string | null | undefined): Promise<void> => {
			const qs = new URLSearchParams({ providerName, providerKey: providerKey ?? '' });
			return await api.request<void>(`${basePath}?${qs.toString()}`, 'DELETE', undefined);
		},
	};
}
