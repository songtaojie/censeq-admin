/** 与 Censeq.FeatureManagement.Application.Contracts 对齐 */

export interface FeatureProviderDto {
	name?: string;
	key?: string;
}

/** 后端为 IStringValueType，序列化后多为带 name / validator 等字段的对象 */
export type FeatureValueTypeJson = Record<string, unknown> | null;

export interface FeatureDto {
	name?: string;
	displayName?: string;
	value?: string;
	description?: string;
	depth?: number;
	parentName?: string;
	valueType?: FeatureValueTypeJson;
	provider?: FeatureProviderDto;
}

export interface FeatureGroupDto {
	name?: string;
	displayName?: string;
	features?: FeatureDto[];
}

export interface GetFeatureListResultDto {
	groups?: FeatureGroupDto[];
}

export interface UpdateFeatureDto {
	name: string;
	value: string;
}

export interface UpdateFeaturesDto {
	features: UpdateFeatureDto[];
}

/** Volo TenantFeatureValueProvider.ProviderName */
export const TENANT_FEATURE_PROVIDER = 'T';
