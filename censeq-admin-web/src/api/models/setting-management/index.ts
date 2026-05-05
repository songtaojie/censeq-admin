/**
 * 与 Censeq.SettingManagement.Application.Contracts 中 DTO 的 JSON（camelCase）字段对齐。
 */

export const SETTING_PROVIDER_NAMES = {
	global: 'G',
	tenant: 'T',
	user: 'U',
} as const;

export type SettingProviderName = (typeof SETTING_PROVIDER_NAMES)[keyof typeof SETTING_PROVIDER_NAMES];

export interface EmailSettingsDto {
	smtpHost?: string | null;
	smtpPort: number;
	smtpUserName?: string | null;
	smtpPassword?: string | null;
	smtpDomain?: string | null;
	smtpEnableSsl: boolean;
	smtpUseDefaultCredentials: boolean;
	defaultFromAddress?: string | null;
	defaultFromDisplayName?: string | null;
}

export interface UpdateEmailSettingsDto {
	smtpHost: string;
	smtpPort: number;
	smtpUserName: string;
	smtpPassword: string;
	smtpDomain: string;
	smtpEnableSsl: boolean;
	smtpUseDefaultCredentials: boolean;
	defaultFromAddress: string;
	defaultFromDisplayName: string;
}

export interface SendTestEmailInput {
	senderEmailAddress: string;
	targetEmailAddress: string;
	subject: string;
	body: string;
}

/** Volo.Abp.NameValue 序列化结果 */
export interface NameValueDto {
	name: string;
	value: string;
}

// ─── SettingDefinition ────────────────────────────────────────────────────────

export interface SettingDefinitionGetListInput {
	filter?: string | null;
	skipCount?: number;
	maxResultCount?: number;
}

export interface SettingDefinitionDto {
	id: string;
	name: string;
	displayName: string;
	description?: string | null;
	defaultValue?: string | null;
	currentValue?: string | null;
	isVisibleToClients: boolean;
	isSystem: boolean;
}

export interface CreateSettingDefinitionDto {
	name: string;
	displayName: string;
	description?: string | null;
	defaultValue?: string | null;
	currentValue?: string | null;
	isVisibleToClients?: boolean;
}

export interface UpdateSettingDefinitionDto {
	displayName: string;
	description?: string | null;
	defaultValue?: string | null;
	currentValue?: string | null;
	isVisibleToClients?: boolean;
}

// ─── SettingValue ─────────────────────────────────────────────────────────────

export interface TenantSettingValueDto {
	tenantId: string;
	value?: string | null;
}

export interface UserSettingValueDto {
	userId: string;
	value?: string | null;
}

export interface SettingValueDto {
	name: string;
	globalValue?: string | null;
	tenantValues: TenantSettingValueDto[];
	userValues: UserSettingValueDto[];
}

export interface SetSettingValueInput {
	name: string;
	value?: string | null;
	/** ProviderName：全局 / 租户 / 用户 */
	providerName: string;
	/** 全局时为 null，租户/用户时为对应实体 ID 字符串 */
	providerKey?: string | null;
}
