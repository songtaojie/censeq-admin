/**
 * 与 Censeq.LocalizationManagement.Application.Contracts 中 DTO 的 JSON（camelCase）字段对齐。
 */

export interface LocalizationResourceDto {
	id: string;
	name: string;
	displayName?: string | null;
	defaultCultureName?: string | null;
	creationTime?: string | null;
	lastModificationTime?: string | null;
}

export interface CreateUpdateLocalizationResourceDto {
	name: string;
	displayName?: string | null;
	defaultCultureName?: string | null;
}

export interface LocalizationCultureDto {
	id: string;
	tenantId?: string | null;
	cultureName: string;
	uiCultureName?: string | null;
	displayName: string;
	isEnabled: boolean;
	creationTime?: string | null;
}

export interface CreateUpdateLocalizationCultureDto {
	cultureName: string;
	displayName: string;
	uiCultureName?: string | null;
	isEnabled: boolean;
}

export interface LocalizationTextDto {
	id: string;
	tenantId?: string | null;
	resourceName: string;
	cultureName: string;
	key: string;
	value?: string | null;
	creationTime?: string | null;
	lastModificationTime?: string | null;
}

export interface CreateLocalizationTextDto {
	resourceName: string;
	cultureName: string;
	key: string;
	value?: string | null;
}

export interface UpdateLocalizationTextDto {
	value?: string | null;
}

export interface GetLocalizationTextsInput {
	resourceName?: string | null;
	cultureName?: string | null;
	filter?: string | null;
	sorting?: string | null;
	skipCount?: number;
	maxResultCount?: number;
}
